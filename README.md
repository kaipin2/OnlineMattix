# Mattix(ボードゲーム)をネット対戦できるようにUnityで制作したときに使用しているスクリプトを保存している

環境：Unity 2021.2.5f1

このゲームは、Mattixというボードゲームをネット対戦
できるように開発したもので、開発環境としては、Unityの
2021.2.5f1を使用し、ネット対戦の環境を作るためにPUN2を、
日本語のテキストを実装するためにTextMeshProを実装しています。

# 1.シーンの役割
 
**Titleシーン**  
ゲームを開始したら一番初めに入るシーン、他のシーンへのパスを多く持ち、「ネット対戦のロビーに入室する」か、「ゲームの説明を見る」か、「ひとりでゲームをする」かを選ぶことができる  

**Lobbyシーン**  
ネット対戦を行うために入るシーン、ここで、既存のルームに入ったり、自分でルームを制作したりすることができる。  

**Explanationシーン**  
ゲームの説明を行うシーン、ここで、Matttixのゲームルールを理解することができる。  

**マティックスシーン**  
実際にMattixを行うシーン、一人で行うときも、二人で行うときも、同じシーンに入り、ゲームが終了したら、Lobbyシーンに戻る。しかし、ゲームを棄権(ゲーム終了ボタンを押す)した場合は、Titleシーンに戻る。

# 2.各スクリプトの役割
## 2.1 Titleシーンのスクリプト
**2.1.1 Title Controller**  
Titleシーンのメインスクリプト。Title画面にボタンを配置し、Actionを設定したり、このゲームのバージョンの表示、BGMを流す命令などを行ったりしている。  

**2.1.2 Photon Manager**  
Photon View とPhoton Transform（PUN2に存在）を持っているObjectに付いているスクリプトで、Titleシーンでは、一人用(Offline)のゲームを開始するときに、マティックスシーンに遷移するための関数を持っているスクリプト。詳しくは、2.2.2で説明している。  

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
今が、ゲーム中かゲーム中でないかを判断し、ゲーム中なら、「ゲーム終了」というボタンを表示するスクリプト。  

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
