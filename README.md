# Mattix(ボードゲーム)をネット対戦できるようにUnityで制作

ソースコードは、
 | Mattixゲーム                        |  外部ツール(PUN2)            |  外部ツール(TexyMeshPro)          |
 | ----                               | ----                  | ----                  | 
 |<img src="https://user-images.githubusercontent.com/86358104/172784169-bd1c4c62-153c-488c-bc51-ba4df1bb2cf0.jpg" />| <img src="https://user-images.githubusercontent.com/86358104/172784215-0a991191-89c5-40d5-b0b0-1a65cd8dcb7a.jpg" />| <img src="https://user-images.githubusercontent.com/86358104/172784206-e7a3b794-7be7-4154-8c84-555abbe50843.jpg" />|
 

環境：Unity 2021.2.5f1  
外部ツール：PUN2（オンライン対戦用のサーバー管理）,TextMeshPro（テキストの表示の幅を広げるツール） 

このゲームは、Mattixというボードゲームをネット対戦
できるように開発したもので、開発環境としては、Unityの
2021.2.5f1を使用し、ネット対戦の環境を作るためにPUN2を、
日本語のテキストを実装するためにTextMeshProを実装しています。

# 0.ゲームの動き

 | Mattixゲーム Youtube動画(クリックで再生)            |
 | ----                                              |
 |[![Mattix](https://img.youtube.com/vi/405UuXExUAc/0.jpg)](https://www.youtube.com/watch?v=405UuXExUAc)|
 
# 1.シーンの役割
 
## **Titleシーン** 

![Title](https://user-images.githubusercontent.com/86358104/172790046-6e830bab-b376-43d1-8bae-77c6b8631a60.png)
![option](https://user-images.githubusercontent.com/86358104/172791809-a9bc6567-65cd-48c9-9d4a-19b83433fce2.png)

ゲームを開始したら一番初めに入るシーン、他のシーンへのパスを多く持ち、「ネット対戦のロビーに入室する」か、「ゲームの説明を見る」か、「ひとりでゲームをする」かを選ぶことができる (右上の歯車を押すと、Option画面が出てくる) 

## **Lobbyシーン**  

![Lobby](https://user-images.githubusercontent.com/86358104/172790957-a42695e3-5351-4cd3-a3ff-149fe4d3eeba.png)

ネット対戦を行うために入るシーン、ここで、既存のルームに入ったり、自分でルームを制作したりすることができる。  

## **Explanationシーン**  

![Explanation](https://user-images.githubusercontent.com/86358104/172791097-a17b7be8-3883-4223-95b6-bb123087da48.png)

ゲームの説明を行うシーン、ここで、Matttixのゲームルールを理解することができる。  

## **マティックスシーン**  

![Mattix](https://user-images.githubusercontent.com/86358104/172791403-eeaa52ce-1d52-4ba9-a8be-272342c9f370.png)
![Finish](https://user-images.githubusercontent.com/86358104/172791663-70700da6-fbc3-4621-aa82-19138d31de62.png)

実際にMattixを行うシーン、一人で行うときも、二人で行うときも、同じシーンに入り、ゲームが終了したら、Lobbyシーンに戻る。しかし、ゲームを棄権(ゲーム終了ボタンを押す)した場合は、Titleシーンに戻る。

# 2.各スクリプトの役割
## 2.1 Titleシーンのスクリプト
**2.1.1 Title Controller**  
Titleシーンのメインスクリプト。Title画面にボタンを配置し、Actionを設定したり、このゲームのバージョンの表示、BGMを流す命令などを行ったりしている。  

## 2.2 Lobbyシーンのスクリプト
**2.2.1 Lobby Controller**  
Lobbyシーンのメインスクリプト。様々なオブジェクトの情報を取得したり、BGMを流す命令をしたりしている。  
 
**2.2.2 Photon Manager**   
ネット接続と、対戦相手とのマッチングするためのスクリプト。Lobbyシーンに存在するテキストに現在の状況(OfflineかOnlineか)などを表示、サーバーに接続、ロビーに入室、ルームに入室などのことを行いその情報も表示している。また、既存のルームが存在する場合、そのルームに入室するためのボタンを表示することも行っている。また、ゲーム開始のボタンを人数が足りている場合に押したときのマティックスシーンに遷移する関数を持っている。  

**2.2.3 PPlayerNameInputField**  
前回使ったプレイヤー名を保存して表示するためのスクリプト  

**2.2.4 PRoomNameInputField**  
前回作成したルーム名を保存して表示するためのスクリプト  

## 2.3 Explanationシーンのスクリプト
**2.3.1 Explanation Controller**  
Explanationシーンのメインスクリプト。Explanationシーンにイメージ画像を表示し、それを変更の支持をしたり、説明文を表示したりするためのスクリプト。  

**2.3.2 Image Disp**  
Explanationシーンにイメージ画像の変更の指示がExplanation Controllerから来た場合に使うスクリプトで、表示する画像の順番を保存し、実際に画像を切り替えるスクリプト。  

## 2.4 マティックスシーンのスクリプト
**2.4.1 Game Controller**  
マティックスシーンのメインスクリプト。Mattixの盤面を表示し、ゲーム全体の流れを制御するスクリプト。
## 2.5 シーンをまたぐスクリプト
**2.5.1 Audio Manager**  
BGMやSEを再生したり、音量の調節をしたりするスクリプト。  

**2.5.2 Sound Presenter**  
BGMの変更や、Sliderの値をAudio Managerに伝えるためのスクリプト  

**2.5.3 Option Manager**  
Optionの画面にて、今が、ゲーム中かゲーム中でないかを判断し、ゲーム中なら、「ゲーム終了」というボタンを表示するスクリプト。  

**2.5.4 OptionStatusController**  
Optionの画面にて、現在のシーンを判断して、追従するカメラを探したり、対戦中(Mattixシーン)ならば、Option画面の盤面の大きさを設定する表示を消したりするスクリプト。
## 2.6 シーン内に登場しないスクリプト
**2.6.1 Board Script**  
マティックスシーンに登場する盤面のコンポーネントとして存在するスクリプトであり、盤面の色の変更や、大きさの変更、オブジェクトの名前の変更などの同期を行っている。  

**2.6.2 Buttons**  
ボタンを押したときのアクションが保存しているスクリプト。ボタンのアクションを設定したい場合は、このスクリプトを使って設定を行う。  

**2.6.3 Button Script**  
ボタンの同期を行うためのスクリプトで、ボタンの色やボタンに書かれているテキスト、親子関係などの変更を同期させるためのスクリプト。  

**2.6.4 Const**  
定数を保存しているスクリプト。保存している定数を使用したい場合は、そのスクリプト内に「using Const;」を実装し、「Const.CO.(使いたい定数)」と表記すれば使用することができる。  

**2.6.5 Enabled**  
ボタンの表示か非表示か判断するためのスクリプト。各ボタンの親(Canvas)が非表示になった時に、そのボタンも非表示にし、逆に表示になった時はボタンも表示させるスクリプト。  

**2.6.6. PointPieceScript**  
ゲームの駒のコンポーネントとして付いているスクリプト。駒の大きさや色や場所、駒の得点の表記、駒を取った時の音の同期を行っている。  

**2.6.7 TextScript**  
テキストの同期を行うためのスクリプト。テキストの書いてある内容や書き方(左詰め、右詰めなど)、色の変更を同期させる  

**2.6.8 ColorSerializer**  
[RPC]で同期を行うときに、引数としてColor型を与えられるようにするスクリプト。GameControllerのスクリプトで使用している。
※[RPC]：PUN2での同期方法の１つ、[PunRPC]属性をつけたメソッドをPhotonViewのRPC()から呼び出すことで、ルーム内の他プレイヤー側でもメソッドを実行することができます。RPC()の第一引数は実行するメソッド名、第二引数はRPCを実行する対象、そして第三引数以降が実行するメソッドの引数です。

| RPCを実行する対象                   |  送信者自身            |  他プレイヤー          |　途中参加者     |
| ----                               | ----                  | ----                  | ----           |
| RpcTarget.All　　                  |  即座に実行される      |  通信を介して実行される |	実行されない     |
| RpcTarget.Others　　               | 実行されない	          | 通信を介して実行される | 実行されない     |
| RpcTarget.AllBuffered　　          |  即座に実行される      | 通信を介して実行される | 実行される       |
| RpcTarget.OthersBuffered          |  実行されない          | 通信を介して実行される | 実行される       |
| RpcTarget.AllViaServer            |  通信を介して実行される | 通信を介して実行される | 実行されない     |
| RpcTarget.AllBufferedViaServer    |  通信を介して実行される | 通信を介して実行される | 実行される       |

| RPCを実行する対象                   |  マスタークライアント（送信者自身） | マスタークライアント（他プレイヤー）| それ以外のプレイヤー |
| ----                               | ----                             | ----                            | ----                |
| RpcTarget.MasterClient             |  即座に実行される                 | 通信を介して実行される             | 実行されない        |
