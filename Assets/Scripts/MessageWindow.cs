using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectXformMover))]
public class MessageWindow : MonoBehaviour
{
    public Image messageImage;

    public Text messageText;
    public Text buttonText;

    public Sprite loseIcon;
    public Sprite winIcon;
    public Sprite goalIcon;

    public Sprite goalCompleteIcon;
    public Sprite goalFailedIcon;

    public Sprite collectIcon;
    public Sprite timerIcon;
    public Sprite movesIcon;

    public Image goalImage;
    public Text goalText;

    public GameObject collectionGoalLayout;

    public void ShowMessage(Sprite sprite = null,
                            string message = "", string buttonMessage = "start")
    {
        if (messageImage != null)
        {
            messageImage.sprite = sprite;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (buttonText != null)
        {
            buttonText.text = buttonMessage;
        }
    }

    public void ShowScoreMessage(int scoreGoal)
    {
        string message = "score goal \n" + scoreGoal.ToString();

        ShowMessage(goalIcon, message, "start");
    }

    public void ShowWinMessage()
    {
        ShowMessage(winIcon, "level\ncomplete", "ok");
    }

    public void ShowLoseMessage()
    {
        ShowMessage(loseIcon, "level\nfailed", "ok");
    }

    public void ShowGoal(string caption = "", Sprite icon = null)
    {
        if (caption != "")
        {
            ShowGoalCaption(caption);
        }

        if (icon != null)
        {
            ShowGoalImage(icon);
        }
    }

    public void ShowGoalCaption(string caption = "", int offsetX = 0, int offsetY = 0)
    {
        if (goalText != null)
        {
            goalText.text = caption;

            RectTransform rectXform = goalText.GetComponent<RectTransform>();
            rectXform.anchoredPosition += new Vector2(offsetX, offsetY);
        }
    }

    public void ShowGoalImage(Sprite icon = null)
    {
        if (goalImage != null)
        {
            goalImage.gameObject.SetActive(true);
            goalImage.sprite = icon;
        }

        if (icon == null)
        {
            goalImage.gameObject.SetActive(false);
        }
    }

    public void ShowTimedGoal(int time)
    {
        string caption = time.ToString() + " seconds";
        ShowGoal(caption, timerIcon);
    }

    public void ShowMovesGoal(int moves)
    {
        string caption = moves.ToString() + " moves";
        ShowGoal(caption, movesIcon);
    }

    public void ShowCollectionGoal(bool state = true)
    {
        if (collectionGoalLayout != null)
        {
            collectionGoalLayout.SetActive(state);
        }

        if (state)
        {
            ShowGoal("", collectIcon);
        }
    }
}
