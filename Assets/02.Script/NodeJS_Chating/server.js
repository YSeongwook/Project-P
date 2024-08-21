const WebSocket = require('ws');

// 웹소켓 서버 생성
const server = new WebSocket.Server({ port: 8080 });

// 모든 연결된 클라이언트를 추적하기 위한 배열
let clients = [];

server.on('connection', (ws) => {
    // 새로운 클라이언트가 연결될 때마다 클라이언트를 배열에 추가
    clients.push(ws);
    console.log('Client connected');

    // 클라이언트가 메시지를 보낼 때 처리
    ws.on('message', (message) => {
        console.log(`Received: ${message}`);
        
        // 모든 클라이언트에게 메시지 브로드캐스트
        clients.forEach(client => {
            if (client.readyState === WebSocket.OPEN) {
                console.log("Sending message to client:", message); // 로그로 전송 메시지 확인
                client.send(message);
            }
        });
    });

    // 클라이언트가 연결을 종료할 때 처리
    ws.on('close', () => {
        console.log('Client disconnected');
        // 연결이 끊긴 클라이언트를 배열에서 제거
        clients = clients.filter(client => client !== ws);
    });
});

console.log('WebSocket server is running on ws://localhost:8080');