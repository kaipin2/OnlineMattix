using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UI���g�p����̂ɕK�v
using TMPro; //TextMeshPro���g�p����̂ɕK�v
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
namespace Const
{
    public static class CO
    {
        //��Ƀ{�^���Ŏg�p����ϐ�
        public static string AudioObjectName = "Audio"; //AudioManager��Component����Ă���Gameobject�̖���
        public static string MainObjectName = "TitleController"; //Title��ʂ�TitleContoroller.cs���i�[����Ă���Object�̖���
        public static string LobbyObjectName = "LobbyController"; //Lobby��ʂ�LobbyContoroller.cs���i�[����Ă���Object�̖���
        public static string PhotonManagerObjectName = "PhotonManager"; //Photon��ʂ�PhotonManager.cs���i�[����Ă���Object�̖���
        public static string GameControllerObjectName = "GameController"; //�}�e�B�b�N�X��ʂ�GameController.cs���i�[����Ă���Object�̖���
        public static string ExplanationControllerObjectName = "ExplanationController"; //������ʂ�ExplanationControllerController.cs���i�[����Ă���Object�̖���

        public static string DisconnectName = "Disconnect"; //�ؒf�{�^���̖��O
        public static string GameStartName = "GameStart"; //�Q�[���J�n�{�^��
        public static string OFFLINEButtonName = "OFFLINEButton"; //OFFLINEButton�̖��O
        public static string ONLINEButtonName = "ONLINEButton"; //ONLINEButton�̖��O
        public static string PlayerNameFieldName = "PlayerNameField"; //PlayerName�̖��O
        public static string RoomNameFieldName = "RoomNameField"; //RoomName�̖��O
        public static string EntertheRoomName = "EntertheRoom"; //EntertheRoom�̖��O
        public static string UpdateButtonName = "UpdateButton"; //UPdateButton�̖��O
        public static string RoomNameName = "RoomName"; //���삳��Ă��镔���ւ̓����{�^����object��
        public static string TitleSceneName = "Title"; //TitleScene�̖��O
        public static string MattixSceneName = "�}�e�B�b�N�X"; //MainScene�̖��O
        public static string LobbySceneName = "Lobby"; //LobbyScene�̖��O
        public static string ExplanationSceneName = "Explanation"; //ExplanationScene�̖��O
        public static string ConfirmationWindowsName = "ConfirmationWindows"; //ConfirmationWindows�̖��O

        //���TitleController�Ŏg�p����ϐ�
        public static string CanvasName = "Canvas"; //Title��ʂ�Canvas�̖���
        public static string AudioCanvasName = "VolumeCanvas"; //Option��Canvas�̖���
        public static string VersionTextName = "Version";
        public static string TitleName = "OptionTitle"; //Title��Text������TextMeshProObject�̖���

        //���AudioManager�Ŏg�p����ϐ�
        public static string BGMFolder = "Audio/BGM"; //BGM�������Ă���t�H���_
        public static string SEFolder = "Audio/SE"; //SE�������Ă���t�H���_

        //���SoundPresenter�Ŏg�p����ϐ�
        public static string bgmNamePrefKey = "BGMVolume";
        public static string seNamePrefKey = "SEVolume";
        public static string Option = "VolumeCanvas/Option";
        public static string BGM = "/BGM";
        public static string SE = "/SE";
        public static string Volume = "/Volume";

        //���LobbyController�Ŏg�p����ϐ�
        public static string PhotonCanvasName = "PhotonCanvas"; //Photon��ʂ�Canvas�̖���

        //���GameController�Ŏg�p����ϐ�
        //gameBoard�Ŏg��Prefab���i�[����Ă���URL
        public static string GameBoardURL = "Prehabs/GameBoard/";
        //NomalButton��Prefab���i�[����Ă���URL
        public static string NomalButtonURL = "Prehabs/";
        public static string NomalButtonURL2 = "Prehabs/NomalButton";
        //�J�����̖���
        public static string MainCamera = "Main Camera";
        //��������u���ʒu������Object�̖���(A:���,B:���)
        public static string FirstGetPositionName = "CubeA";
        public static string SecondGetPositionName = "CubeB";
        //��肪��Ԃ̎��Ɏ����e�L�X�g�\��
        public static string FirstTrunText = "��U(��)�̔Ԃł�";
        public static string FirstPlayerText = "���Ȃ��͐��ł�";
        //��肪��Ԃ̎��Ɏ����e�L�X�g�\��
        public static string SecondTrunText = "��U(��)�̔Ԃł�";
        public static string SecondPlayerText = "���Ȃ��͌��ł�";
        //���K���[�h�̎��̃e�L�X�g
        public static string OnePlayerText = "��l�p���[�h";
        //�Q�[�����I�������Ƃ���(��Ԃ�\������)�e�L�X�g
        public static string Finish = "�Q�[���I��";
        //���������Ƃ��̃e�L�X�g
        public static string WINText = "WIN";
        //���������̎��̃e�L�X�g
        public static string DRAW = "DRAW";
        //�e�L�X�g���E�񂹂���Ƃ�(ChangeAlignment()�̓���)
        public static string Right = "Right";
        //�e�L�X�g�����񂹂���Ƃ�(ChangeAlignment()�̓���)
        public static string Left = "Left";
        //�e�L�X�g�𒆉��񂹂���Ƃ�(ChangeAlignment()�̓���)
        public static string Center = "Center";
        //��̓�����Object��URL
        public static string PointInsideURL = "inside";
        //�|�C���g��̃|�C���g����\������e�L�X�g��Object��URL
        public static string PointNumberURL = "inside/Point";
        //�F��h��ꏊ(0:�Ֆʂ̐F�h��,1:��̊O���̐F�h��,2:��̓����̐F�h��)
        public static int BoardNumber = 0;
        public static int PointNumber = 1;
        public static int InsideNumber = 2;
        //�Q�[���Ԃ̑傫��
        public static int Ver = 6; //�c
        public static int Si = 6; //��
        //��̓_���̏���Ɖ���
        public static int Max = 10; //���
        public static int Min = -10; //����
        //EMPTY=0,MAIN=30�Œ�`
        public static int EMPTY = 0;
        public static int MAIN = Max * 3;
        //��Ԃ̃v���C���[������(SIDE:���,VERTICAL:���)
        public static int VERTICAL = 2;
        public static int SIDE = -2;
        //�Q�[���Ԃ̖��O( (BoardName) + (x���W) + (BoardName_Connect) + (y���W) �ƂȂ�)
        public static string BoardName = "Cube";
        public static string BoardName_Connect = "_";

        //���PhotonManager�Ŏg�p����ϐ�
        public static int button_Ver = 3; //�{�^���̕���(�c�̌�)
        public static int button_Side = 3; //�{�^���̕���(���̌�)
        public static int Number = button_Ver * button_Side; //���[���̍ő吧��\��
        public static string RoomButtonParent = "PhotonCanvas/Room";//���[���{�^���̈ʒu��URL
        public static string StatusTextName = "Title";
        public static string MessageTextName = "Message";
        public static string LobbyTitle = "���r�[";
        public static string GameVersion = "Ver 1.1.0"; //�Q�[���o�[�W�����w��i�ݒ肵�Ȃ��ƌx�����o��j

    }
}
