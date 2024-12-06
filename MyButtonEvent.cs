using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; // HTTPリクエスト用
using System.Collections; // IEnumerator用

public class ButtonCounter : MonoBehaviour
{
    [SerializeField]
    private Button _button; // UIのボタン
    [SerializeField]
    private TextMeshProUGUI _label; //カウント表示用のラベル

    private int _count = 0;
    private string esp32Url = "http://192.168.4.1:80/"; // ESP32のIPアドレス
    private string esp32SendUrl = "http://192.168.4.1:80/counter";


    private void Start()
    {
        //初期化: ボタンのクリックイベントにメソッドを登録
        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        //初期表示
        UpdateLabel();
    }

    private void OnButtonClicked()
    {
        //  カウントを増加
        _count++;

        // ラベルを更新
        UpdateLabel();

        // サーバーに接続できるかを確認
        StartCoroutine(CheckServerConnection());
    }

    private void UpdateLabel()
    {
        if (_label != null)
        {
            _label.text = $"カウント: {_count}";
        }
    }

    private void SendDataToESP32()
    {
        StartCoroutine(SendRequest());
    }

    
    private IEnumerator SendRequest() //コルーチン
    {
        // 送信データをURLに付加
        string requestUrl = $"{esp32SendUrl}?count={_count}"; //string...文字列　『string 変数 = “文字”』のように使う

        using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
        {
            yield return request.SendWebRequest();//HTTPリクエストが終わるまで待つ

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error sending data: {request.error}");
            }
            else
            {
                Debug.Log($"Data sent successfully: {_count}");
            }
        }
    }

    private IEnumerator CheckServerConnection()//コルーチン
    {
        using (UnityWebRequest request = UnityWebRequest.Get(esp32Url))
        {
            // 接続確認　非同期でHTTPリクエストを送信
            yield return request.SendWebRequest();//HTTPリクエストが終わるまで待つ

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                // 接続失敗
                Debug.LogError("Cannot connect to ESP32 server. Please check the network connection.");
            }
            else
            {
                // 接続成功
                Debug.Log("Connected to ESP32 server successfully.");
                SendDataToESP32(); // 接続成功ならデータ送信
            }
        }
    }
}
