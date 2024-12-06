#include <Arduino.h>
#include <Arduino.h>
#include <WiFi.h>
#include <WebServer.h>

// アクセスポイントの設定
const char* ssid = "ESP32_AP";
const char* password = "00000000";

// Webサーバーのインスタンスを作成
WebServer server(80);

// カウントの格納変数
int count = 0;

// ハンドラ関数: カウントを受信
void handleCounter() {
  Serial.printf("a");
   if (server.hasArg("count")) { // "count" パラメータがあるか確認
    count = server.arg("count").toInt(); // パラメータの値を整数に変換
    Serial.printf("Received count: %d\n", count); // シリアルモニタに表示
    server.send(200, "text/plain", "Count received successfully!"); // クライアントに応答
  } else {
    server.send(400, "text/plain", "Error: Missing 'count' parameter"); // パラメータが無ければエラー応答
  }
}

// ハンドラ関数: ルートページ
void handleRoot() {
 String message = "ESP32 is running!"; // 応答メッセージ
 server.send(200, "text/plain", message); // クライアントに応答
 Serial.printf("Unity has connected \n");
}

void setup() {
  Serial.begin(9600);
  delay(1000);

  // アクセスポイントを開始
  Serial.println("Starting Access Point...");
  WiFi.softAP(ssid, password);

  // IPアドレスを表示
  Serial.print("AP IP address: ");
  Serial.println(WiFi.softAPIP());

  // ルートハンドラの設定
  server.on("/", handleRoot);

  // カウントデータのハンドラ設定
  server.on("/counter", handleCounter);

  // サーバー開始
  server.begin();
  Serial.println("HTTP server started");
}

void loop() {
  server.handleClient();
}
