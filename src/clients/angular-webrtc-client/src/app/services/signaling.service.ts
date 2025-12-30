import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';

export interface PeerInfo {
  connectionId: string;
  peerId: string;
  displayName: string;
  joinedAt: string;
}

export interface OfferMessage {
  fromPeerId: string;
  fromDisplayName: string;
  sdpOffer: string;
}

export interface AnswerMessage {
  fromPeerId: string;
  sdpAnswer: string;
}

export interface IceCandidateMessage {
  fromPeerId: string;
  candidate: string;
  sdpMid: string;
  sdpMLineIndex: number;
}

@Injectable({
  providedIn: 'root'
})
export class SignalingService {
  private hubConnection: HubConnection | null = null;
  private hubUrl = 'https://localhost:5001/hubs/signaling';

  // Observables
  private connectedSubject = new BehaviorSubject<boolean>(false);
  private peersSubject = new BehaviorSubject<PeerInfo[]>([]);
  private peerJoinedSubject = new Subject<PeerInfo>();
  private peerLeftSubject = new Subject<string>();
  private offerReceivedSubject = new Subject<OfferMessage>();
  private answerReceivedSubject = new Subject<AnswerMessage>();
  private iceCandidateReceivedSubject = new Subject<IceCandidateMessage>();
  private errorSubject = new Subject<string>();

  connected$ = this.connectedSubject.asObservable();
  peers$ = this.peersSubject.asObservable();
  peerJoined$ = this.peerJoinedSubject.asObservable();
  peerLeft$ = this.peerLeftSubject.asObservable();
  offerReceived$ = this.offerReceivedSubject.asObservable();
  answerReceived$ = this.answerReceivedSubject.asObservable();
  iceCandidateReceived$ = this.iceCandidateReceivedSubject.asObservable();
  error$ = this.errorSubject.asObservable();

  async connect(): Promise<void> {
    if (this.hubConnection) {
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    this.registerHandlers();

    try {
      await this.hubConnection.start();
      this.connectedSubject.next(true);
      console.log('Connected to signaling hub');
    } catch (err) {
      console.error('Error connecting to signaling hub:', err);
      throw err;
    }
  }

  async disconnect(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.hubConnection = null;
      this.connectedSubject.next(false);
      this.peersSubject.next([]);
    }
  }

  private registerHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('PeersList', (peers: PeerInfo[]) => {
      this.peersSubject.next(peers);
    });

    this.hubConnection.on('PeerJoined', (peer: PeerInfo) => {
      const currentPeers = this.peersSubject.value;
      this.peersSubject.next([...currentPeers, peer]);
      this.peerJoinedSubject.next(peer);
    });

    this.hubConnection.on('PeerLeft', (peerId: string) => {
      const currentPeers = this.peersSubject.value;
      this.peersSubject.next(currentPeers.filter(p => p.peerId !== peerId));
      this.peerLeftSubject.next(peerId);
    });

    this.hubConnection.on('OfferReceived', (offer: OfferMessage) => {
      this.offerReceivedSubject.next(offer);
    });

    this.hubConnection.on('AnswerReceived', (answer: AnswerMessage) => {
      this.answerReceivedSubject.next(answer);
    });

    this.hubConnection.on('IceCandidateReceived', (candidate: IceCandidateMessage) => {
      this.iceCandidateReceivedSubject.next(candidate);
    });

    this.hubConnection.on('Error', (error: string) => {
      this.errorSubject.next(error);
    });

    this.hubConnection.onreconnecting(() => {
      console.log('Reconnecting to signaling hub...');
    });

    this.hubConnection.onreconnected(() => {
      console.log('Reconnected to signaling hub');
      this.connectedSubject.next(true);
    });

    this.hubConnection.onclose(() => {
      console.log('Disconnected from signaling hub');
      this.connectedSubject.next(false);
    });
  }

  async register(peerId: string, displayName: string): Promise<void> {
    if (!this.hubConnection) throw new Error('Not connected');
    await this.hubConnection.invoke('Register', peerId, displayName);
  }

  async sendOffer(targetPeerId: string, sdpOffer: string): Promise<void> {
    if (!this.hubConnection) throw new Error('Not connected');
    await this.hubConnection.invoke('SendOffer', targetPeerId, sdpOffer);
  }

  async sendAnswer(targetPeerId: string, sdpAnswer: string): Promise<void> {
    if (!this.hubConnection) throw new Error('Not connected');
    await this.hubConnection.invoke('SendAnswer', targetPeerId, sdpAnswer);
  }

  async sendIceCandidate(targetPeerId: string, candidate: string, sdpMid: string, sdpMLineIndex: number): Promise<void> {
    if (!this.hubConnection) throw new Error('Not connected');
    await this.hubConnection.invoke('SendIceCandidate', targetPeerId, candidate, sdpMid, sdpMLineIndex);
  }

  async getPeers(): Promise<PeerInfo[]> {
    if (!this.hubConnection) throw new Error('Not connected');
    return await this.hubConnection.invoke('GetPeers');
  }
}
