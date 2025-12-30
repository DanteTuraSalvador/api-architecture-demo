import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { SignalingService, IceCandidateMessage, OfferMessage, AnswerMessage } from './signaling.service';

export interface PeerConnection {
  peerId: string;
  displayName: string;
  connection: RTCPeerConnection;
  dataChannel: RTCDataChannel | null;
  localStream: MediaStream | null;
  remoteStream: MediaStream | null;
}

export interface ChatMessage {
  from: string;
  text: string;
  timestamp: Date;
  isLocal: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class WebrtcService {
  private peerConnections = new Map<string, PeerConnection>();
  private localStream: MediaStream | null = null;
  private myPeerId = '';
  private myDisplayName = '';

  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  private remoteStreamSubject = new Subject<{ peerId: string; stream: MediaStream }>();
  private connectionStateSubject = new Subject<{ peerId: string; state: string }>();

  messages$ = this.messagesSubject.asObservable();
  remoteStream$ = this.remoteStreamSubject.asObservable();
  connectionState$ = this.connectionStateSubject.asObservable();

  private rtcConfig: RTCConfiguration = {
    iceServers: [
      { urls: 'stun:stun.l.google.com:19302' },
      { urls: 'stun:stun1.l.google.com:19302' }
    ]
  };

  constructor(private signalingService: SignalingService) {
    this.setupSignalingHandlers();
  }

  private setupSignalingHandlers(): void {
    this.signalingService.offerReceived$.subscribe(offer => {
      this.handleOffer(offer);
    });

    this.signalingService.answerReceived$.subscribe(answer => {
      this.handleAnswer(answer);
    });

    this.signalingService.iceCandidateReceived$.subscribe(candidate => {
      this.handleIceCandidate(candidate);
    });

    this.signalingService.peerLeft$.subscribe(peerId => {
      this.closePeerConnection(peerId);
    });
  }

  async initialize(peerId: string, displayName: string): Promise<void> {
    this.myPeerId = peerId;
    this.myDisplayName = displayName;
    await this.signalingService.connect();
    await this.signalingService.register(peerId, displayName);
  }

  async getLocalStream(): Promise<MediaStream> {
    if (!this.localStream) {
      this.localStream = await navigator.mediaDevices.getUserMedia({
        video: true,
        audio: true
      });
    }
    return this.localStream;
  }

  async call(targetPeerId: string, displayName: string): Promise<void> {
    const pc = this.createPeerConnection(targetPeerId, displayName);

    // Create data channel for chat
    const dataChannel = pc.connection.createDataChannel('chat');
    this.setupDataChannel(dataChannel, targetPeerId);
    pc.dataChannel = dataChannel;

    // Add local tracks if we have a stream
    if (this.localStream) {
      this.localStream.getTracks().forEach(track => {
        pc.connection.addTrack(track, this.localStream!);
      });
    }

    // Create and send offer
    const offer = await pc.connection.createOffer();
    await pc.connection.setLocalDescription(offer);
    await this.signalingService.sendOffer(targetPeerId, JSON.stringify(offer));
  }

  private createPeerConnection(peerId: string, displayName: string): PeerConnection {
    const pc = new RTCPeerConnection(this.rtcConfig);

    const peerConnection: PeerConnection = {
      peerId,
      displayName,
      connection: pc,
      dataChannel: null,
      localStream: this.localStream,
      remoteStream: null
    };

    pc.onicecandidate = (event) => {
      if (event.candidate) {
        this.signalingService.sendIceCandidate(
          peerId,
          JSON.stringify(event.candidate),
          event.candidate.sdpMid || '',
          event.candidate.sdpMLineIndex || 0
        );
      }
    };

    pc.ontrack = (event) => {
      if (!peerConnection.remoteStream) {
        peerConnection.remoteStream = new MediaStream();
      }
      event.streams[0].getTracks().forEach(track => {
        peerConnection.remoteStream!.addTrack(track);
      });
      this.remoteStreamSubject.next({ peerId, stream: peerConnection.remoteStream });
    };

    pc.ondatachannel = (event) => {
      this.setupDataChannel(event.channel, peerId);
      peerConnection.dataChannel = event.channel;
    };

    pc.onconnectionstatechange = () => {
      this.connectionStateSubject.next({ peerId, state: pc.connectionState });
    };

    this.peerConnections.set(peerId, peerConnection);
    return peerConnection;
  }

  private setupDataChannel(dataChannel: RTCDataChannel, peerId: string): void {
    dataChannel.onmessage = (event) => {
      const message: ChatMessage = {
        from: peerId,
        text: event.data,
        timestamp: new Date(),
        isLocal: false
      };
      const messages = this.messagesSubject.value;
      this.messagesSubject.next([...messages, message]);
    };

    dataChannel.onopen = () => {
      console.log(`Data channel opened with ${peerId}`);
    };

    dataChannel.onclose = () => {
      console.log(`Data channel closed with ${peerId}`);
    };
  }

  private async handleOffer(offer: OfferMessage): Promise<void> {
    const pc = this.createPeerConnection(offer.fromPeerId, offer.fromDisplayName);

    if (this.localStream) {
      this.localStream.getTracks().forEach(track => {
        pc.connection.addTrack(track, this.localStream!);
      });
    }

    const sdpOffer = JSON.parse(offer.sdpOffer);
    await pc.connection.setRemoteDescription(new RTCSessionDescription(sdpOffer));

    const answer = await pc.connection.createAnswer();
    await pc.connection.setLocalDescription(answer);

    await this.signalingService.sendAnswer(offer.fromPeerId, JSON.stringify(answer));
  }

  private async handleAnswer(answer: AnswerMessage): Promise<void> {
    const pc = this.peerConnections.get(answer.fromPeerId);
    if (!pc) return;

    const sdpAnswer = JSON.parse(answer.sdpAnswer);
    await pc.connection.setRemoteDescription(new RTCSessionDescription(sdpAnswer));
  }

  private async handleIceCandidate(message: IceCandidateMessage): Promise<void> {
    const pc = this.peerConnections.get(message.fromPeerId);
    if (!pc) return;

    const candidate = JSON.parse(message.candidate);
    await pc.connection.addIceCandidate(new RTCIceCandidate(candidate));
  }

  sendMessage(peerId: string, text: string): void {
    const pc = this.peerConnections.get(peerId);
    if (pc?.dataChannel?.readyState === 'open') {
      pc.dataChannel.send(text);

      const message: ChatMessage = {
        from: this.myDisplayName,
        text,
        timestamp: new Date(),
        isLocal: true
      };
      const messages = this.messagesSubject.value;
      this.messagesSubject.next([...messages, message]);
    }
  }

  broadcastMessage(text: string): void {
    this.peerConnections.forEach((pc, peerId) => {
      if (pc.dataChannel?.readyState === 'open') {
        pc.dataChannel.send(text);
      }
    });

    const message: ChatMessage = {
      from: this.myDisplayName,
      text,
      timestamp: new Date(),
      isLocal: true
    };
    const messages = this.messagesSubject.value;
    this.messagesSubject.next([...messages, message]);
  }

  private closePeerConnection(peerId: string): void {
    const pc = this.peerConnections.get(peerId);
    if (pc) {
      pc.dataChannel?.close();
      pc.connection.close();
      this.peerConnections.delete(peerId);
    }
  }

  cleanup(): void {
    this.peerConnections.forEach((pc) => {
      pc.dataChannel?.close();
      pc.connection.close();
    });
    this.peerConnections.clear();

    if (this.localStream) {
      this.localStream.getTracks().forEach(track => track.stop());
      this.localStream = null;
    }

    this.signalingService.disconnect();
  }
}
