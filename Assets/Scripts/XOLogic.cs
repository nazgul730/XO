using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XOLogic : MonoBehaviour
{
    public float TilesOffset;

    [SerializeField]
    private Text TurnInformer;
    [SerializeField]
    private RectTransform GameField;
    [SerializeField]
    private float TileHeight;
    [SerializeField]
    private float TileWidth;
    [SerializeField]
    private Button TileTemplate;
    [SerializeField]
    private RectTransform WinnerCongratsPanel;
    [SerializeField]
    private Text WinnerCountText;
    [SerializeField]
    private Text XCounter;
    [SerializeField]
    private Text OCounter;

    private Button[] Tiles = new Button[9];
    private byte[,] FieldGrid = new byte[3, 3];
    private byte Horizontal;
    private byte Vertical;
    private byte LastHorizontal;
    private byte LastVertical;
    private PlayableSign PlayableSign;

    private int WinCountX = 0;
    private int WinCountO = 0;

    void Start()
    {
        InitializeTiles();
        SetFieldToDefaultState();
    }

    void InitializeTiles()
    {        
        float posX = -TileWidth * 3 / 2 - TileWidth / 2;
        float posY = Mathf.Abs(TileWidth);
        float Xreset = posX;
        int i = 0;
        for (int y = 0; y < 3; y++)
        {
            posY -= TileHeight;
            for (int x = 0; x < 3; x++)
            {
                posX += TileWidth;
                Tiles[i] = Instantiate(TileTemplate);
                RectTransform b = Tiles[i].GetComponent<RectTransform>();
                b.SetParent(GameField);
                b.localScale = Vector3.one;
                b.anchoredPosition = new Vector2(posX, posY);
                b.gameObject.name = "Button_ID_" + i;
                i++;
            }
            posX = Xreset;
        }
        TileTemplate.gameObject.SetActive(false);
    }

    public void SetFieldToDefaultState()
    {
        Vertical = 0;
        Horizontal = 0;
        LastHorizontal = 100;
        LastVertical = 100;
        PlayableSign = (PlayableSign)Random.Range(0, 2);
        SetInfo(PlayableSign);
        foreach (Button b in Tiles)
        {
            b.GetComponentInChildren<Text>().text = string.Empty;
            b.interactable = true;
        }
        ResetButton();
    }

    void SetInfo(PlayableSign sign)
    {
        TurnInformer.text = sign.ToString();
    }

    void ResetButton()
    {
        byte i = 0;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                Tiles[i].onClick.RemoveAllListeners();
                Button tmpButton = Tiles[i];
                byte tmp = (byte)(-(i + 1));
                Tiles[i].onClick.AddListener(() => { SetGrid(tmp); SetButtonText(tmpButton); });
                FieldGrid[y, x] = tmp;
                i++;
            }
        }
    }

    void SetGrid(byte value)
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (FieldGrid[y, x] == value)
                {
                    FieldGrid[y, x] = (byte)PlayableSign;
                }
            }
        }

        CheckResult();
    }

    void SetButtonText(Button button)
    {
        button.GetComponentInChildren<Text>().text = TurnInformer.text;

        PlayableSign = PlayableSign == PlayableSign.X ? PlayableSign.O : PlayableSign.X;

        SetInfo(PlayableSign);
        button.interactable = false;
    }

    void CheckResult()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                ByHorizontal(FieldGrid[y, x]);
                ByVertical(FieldGrid[x, y]);
            }
            LastHorizontal = 100;
            LastVertical = 100;
            Vertical = 0;
            Horizontal = 0;
        }

        if (FieldGrid[0, 0] == 0 && FieldGrid[1, 1] == 0 && FieldGrid[2, 2] == 0 
            || FieldGrid[0, 0] == 1 && FieldGrid[1, 1] == 1 && FieldGrid[2, 2] == 1
            || FieldGrid[2, 0] == 0 && FieldGrid[1, 1] == 0 && FieldGrid[0, 2] == 0 
            || FieldGrid[2, 0] == 1 && FieldGrid[1, 1] == 1 && FieldGrid[0, 2] == 1)
        {
            Winner();
        }
    }

    void ByHorizontal(byte value)
    {
        if (value == 0 && LastHorizontal == 100 
            || value == 1 && LastHorizontal == 100)
        {
            LastHorizontal = value;
            Horizontal++;
        }
        else if (value == LastHorizontal)
        {
            Horizontal++;
        }
        else
        {
            Horizontal = 0;
        }

        if (Horizontal == 3)
        {
            Winner();
        }
    }

    void ByVertical(byte value)
    {
        if (value == 0 && LastVertical == 100 
            || value == 1 && LastVertical == 100)
        {
            LastVertical = value;
            Vertical++;
        }
        else if (LastVertical == value)
        {
            Vertical++;
        }
        else
        {
            Vertical = 0;
        }

        if (Vertical == 3)
        {
            Winner();
        }
    }

    void Winner()
    {
        WinnerCountText.text = $"{PlayableSign} wins!";

        if (PlayableSign == PlayableSign.X)
            WinCountX++;
        else
            WinCountO++;

        XCounter.text = $"{WinCountX}";
        OCounter.text = $"{WinCountO}";

        foreach (Button b in Tiles)
        {
            b.interactable = false;
        }

        WinnerCongratsPanel.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum PlayableSign
{
    X=0,
    O=1
}
