import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SignalingService, PeerInfo } from './services/signaling.service';
import { WebrtcService, ChatMessage } from './services/webrtc.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="app">
      <header class="app-header">
        <h1>WebRTC Video Chat</h1>
        <p>Peer-to-peer communication using WebRTC with SignalR signaling</p>
      </header>

      <main class="app-main">
        <!-- Connection Panel -->
        <div class="connection-panel" *ngIf="!isConnected">
          <h2>Join the Room</h2>
          <div class="form-group">
            <label for="displayName">Your Name</label>
            <input
              id="displayName"
              type="text"
              [(ngModel)]="displayName"
              placeholder="Enter your name..."
            />
          </div>
          <button class="btn btn-primary" (click)="connect()" [disabled]="!displayName">
            Join Room
          </button>
        </div>

        <!-- Main Content -->
        <div class="main-content" *ngIf="isConnected">
          <div class="video-section">
            <div class="video-grid">
              <div class="video-container local">
                <video #localVideo autoplay muted playsinline></video>
                <span class="video-label">You ({{ displayName }})</span>
              </div>
              <div class="video-container remote" *ngFor="let stream of remoteStreams">
                <video [srcObject]="stream.stream" autoplay playsinline></video>
                <span class="video-label">{{ getPeerName(stream.peerId) }}</span>
              </div>
            </div>

            <div class="controls">
              <button class="btn" (click)="toggleVideo()" [class.active]="videoEnabled">
                {{ videoEnabled ? 'Disable Video' : 'Enable Video' }}
              </button>
              <button class="btn" (click)="toggleAudio()" [class.active]="audioEnabled">
                {{ audioEnabled ? 'Mute' : 'Unmute' }}
              </button>
              <button class="btn btn-danger" (click)="disconnect()">
                Leave Room
              </button>
            </div>
          </div>

          <aside class="sidebar">
            <!-- Peers List -->
            <div class="panel">
              <h3>Participants ({{ peers.length + 1 }})</h3>
              <ul class="peers-list">
                <li class="peer-item me">
                  <span class="peer-name">{{ displayName }} (You)</span>
                  <span class="status online">Online</span>
                </li>
                <li class="peer-item" *ngFor="let peer of peers">
                  <span class="peer-name">{{ peer.displayName }}</span>
                  <button
                    class="btn btn-sm"
                    (click)="callPeer(peer)"
                    *ngIf="!isConnectedTo(peer.peerId)"
                  >
                    Call
                  </button>
                  <span class="status connected" *ngIf="isConnectedTo(peer.peerId)">
                    Connected
                  </span>
                </li>
              </ul>
              <p class="empty-state" *ngIf="peers.length === 0">
                Waiting for others to join...
              </p>
            </div>

            <!-- Chat -->
            <div class="panel chat-panel">
              <h3>Chat</h3>
              <div class="chat-messages" #chatMessages>
                <div
                  class="message"
                  *ngFor="let msg of messages"
                  [class.local]="msg.isLocal"
                >
                  <span class="message-from">{{ msg.from }}</span>
                  <span class="message-text">{{ msg.text }}</span>
                  <span class="message-time">
                    {{ msg.timestamp | date:'shortTime' }}
                  </span>
                </div>
                <p class="empty-state" *ngIf="messages.length === 0">
                  No messages yet
                </p>
              </div>
              <div class="chat-input">
                <input
                  type="text"
                  [(ngModel)]="messageText"
                  placeholder="Type a message..."
                  (keyup.enter)="sendMessage()"
                />
                <button class="btn btn-primary" (click)="sendMessage()">
                  Send
                </button>
              </div>
            </div>
          </aside>
        </div>
      </main>

      <footer class="app-footer">
        <p>
          Part of the Multi-Protocol API Architecture Demo<br>
          REST | GraphQL | gRPC | SignalR | SSE | MQTT | <strong>WebRTC</strong>
        </p>
      </footer>
    </div>
  `,
  styles: [`
    * { box-sizing: border-box; margin: 0; padding: 0; }

    .app {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      background: #f3f4f6;
    }

    .app-header {
      background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);
      color: white;
      padding: 1.5rem;
      text-align: center;
    }

    .app-header h1 { font-size: 1.75rem; margin-bottom: 0.25rem; }
    .app-header p { opacity: 0.9; }

    .app-main { flex: 1; padding: 1.5rem; }

    .connection-panel {
      max-width: 400px;
      margin: 3rem auto;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .connection-panel h2 { margin-bottom: 1.5rem; color: #333; }

    .form-group {
      margin-bottom: 1rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
    }

    .form-group input {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #d1d5db;
      border-radius: 8px;
      font-size: 1rem;
    }

    .main-content {
      display: grid;
      grid-template-columns: 1fr 350px;
      gap: 1.5rem;
      max-width: 1400px;
      margin: 0 auto;
    }

    .video-section {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .video-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .video-container {
      position: relative;
      background: #1f2937;
      border-radius: 8px;
      overflow: hidden;
      aspect-ratio: 16/9;
    }

    .video-container video {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .video-label {
      position: absolute;
      bottom: 0.5rem;
      left: 0.5rem;
      background: rgba(0,0,0,0.7);
      color: white;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
    }

    .controls {
      display: flex;
      justify-content: center;
      gap: 0.5rem;
    }

    .sidebar { display: flex; flex-direction: column; gap: 1rem; }

    .panel {
      background: white;
      border-radius: 12px;
      padding: 1rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .panel h3 { margin-bottom: 1rem; color: #333; font-size: 1rem; }

    .peers-list { list-style: none; }

    .peer-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.75rem;
      border-bottom: 1px solid #f3f4f6;
    }

    .peer-item.me { background: #f9fafb; }

    .status {
      font-size: 0.75rem;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
    }

    .status.online { background: #dcfce7; color: #166534; }
    .status.connected { background: #dbeafe; color: #1e40af; }

    .chat-panel { flex: 1; display: flex; flex-direction: column; }

    .chat-messages {
      flex: 1;
      max-height: 300px;
      overflow-y: auto;
      padding: 0.5rem;
      background: #f9fafb;
      border-radius: 8px;
      margin-bottom: 0.5rem;
    }

    .message {
      padding: 0.5rem;
      margin-bottom: 0.5rem;
      border-radius: 8px;
      background: white;
    }

    .message.local { background: #dbeafe; }

    .message-from { font-weight: 600; font-size: 0.75rem; color: #6b7280; }
    .message-text { display: block; margin: 0.25rem 0; }
    .message-time { font-size: 0.625rem; color: #9ca3af; }

    .chat-input { display: flex; gap: 0.5rem; }
    .chat-input input { flex: 1; padding: 0.5rem; border: 1px solid #d1d5db; border-radius: 8px; }

    .btn {
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 8px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
      background: #e5e7eb;
    }

    .btn:hover { background: #d1d5db; }
    .btn-primary { background: #8b5cf6; color: white; }
    .btn-primary:hover { background: #7c3aed; }
    .btn-danger { background: #ef4444; color: white; }
    .btn-danger:hover { background: #dc2626; }
    .btn-sm { padding: 0.25rem 0.5rem; font-size: 0.75rem; }
    .btn.active { background: #22c55e; color: white; }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }

    .empty-state { color: #9ca3af; text-align: center; padding: 1rem; font-size: 0.875rem; }

    .app-footer {
      background: #1f2937;
      color: #9ca3af;
      text-align: center;
      padding: 1rem;
      font-size: 0.875rem;
    }

    .app-footer strong { color: #8b5cf6; }

    @media (max-width: 1024px) {
      .main-content { grid-template-columns: 1fr; }
    }
  `]
})
export class AppComponent implements OnInit, OnDestroy {
  @ViewChild('localVideo') localVideoRef!: ElementRef<HTMLVideoElement>;
  @ViewChild('chatMessages') chatMessagesRef!: ElementRef<HTMLDivElement>;

  displayName = '';
  messageText = '';
  isConnected = false;
  peers: PeerInfo[] = [];
  messages: ChatMessage[] = [];
  remoteStreams: { peerId: string; stream: MediaStream }[] = [];
  connectedPeers: Set<string> = new Set();
  videoEnabled = true;
  audioEnabled = true;

  private subscriptions: Subscription[] = [];
  private peerId = '';

  constructor(
    private signalingService: SignalingService,
    private webrtcService: WebrtcService
  ) {}

  ngOnInit(): void {
    this.subscriptions.push(
      this.signalingService.peers$.subscribe(peers => {
        this.peers = peers;
      }),
      this.webrtcService.messages$.subscribe(messages => {
        this.messages = messages;
        setTimeout(() => this.scrollChatToBottom(), 100);
      }),
      this.webrtcService.remoteStream$.subscribe(({ peerId, stream }) => {
        const existing = this.remoteStreams.find(s => s.peerId === peerId);
        if (!existing) {
          this.remoteStreams.push({ peerId, stream });
        }
        this.connectedPeers.add(peerId);
      }),
      this.webrtcService.connectionState$.subscribe(({ peerId, state }) => {
        if (state === 'connected') {
          this.connectedPeers.add(peerId);
        } else if (state === 'disconnected' || state === 'failed') {
          this.connectedPeers.delete(peerId);
          this.remoteStreams = this.remoteStreams.filter(s => s.peerId !== peerId);
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
    this.webrtcService.cleanup();
  }

  async connect(): Promise<void> {
    this.peerId = crypto.randomUUID();
    await this.webrtcService.initialize(this.peerId, this.displayName);
    this.isConnected = true;

    // Get local stream and display it
    try {
      const stream = await this.webrtcService.getLocalStream();
      setTimeout(() => {
        if (this.localVideoRef) {
          this.localVideoRef.nativeElement.srcObject = stream;
        }
      }, 100);
    } catch (err) {
      console.error('Could not get media devices:', err);
    }
  }

  disconnect(): void {
    this.webrtcService.cleanup();
    this.isConnected = false;
    this.peers = [];
    this.messages = [];
    this.remoteStreams = [];
    this.connectedPeers.clear();
  }

  async callPeer(peer: PeerInfo): Promise<void> {
    await this.webrtcService.call(peer.peerId, peer.displayName);
  }

  isConnectedTo(peerId: string): boolean {
    return this.connectedPeers.has(peerId);
  }

  getPeerName(peerId: string): string {
    const peer = this.peers.find(p => p.peerId === peerId);
    return peer?.displayName || peerId.substring(0, 8);
  }

  sendMessage(): void {
    if (this.messageText.trim()) {
      this.webrtcService.broadcastMessage(this.messageText);
      this.messageText = '';
    }
  }

  toggleVideo(): void {
    // Implementation would toggle video track
    this.videoEnabled = !this.videoEnabled;
  }

  toggleAudio(): void {
    // Implementation would toggle audio track
    this.audioEnabled = !this.audioEnabled;
  }

  private scrollChatToBottom(): void {
    if (this.chatMessagesRef) {
      const el = this.chatMessagesRef.nativeElement;
      el.scrollTop = el.scrollHeight;
    }
  }
}
