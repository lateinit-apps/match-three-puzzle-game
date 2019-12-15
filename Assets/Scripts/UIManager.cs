using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject collectionGoalLayout;

    public int collectionGoalBaseWidth = 125;

    private CollectionGoalPanel[] collectionGoalPanels;

    public ScreenFader screenFader;

    public Text levelNameText;
    public Text movesLeftText;

    public MessageWindow messageWindow;
    public ScoreMeter scoreMeter;

    public GameObject movesCounter;

    public Timer timer;

    public override void Awake()
    {
        base.Awake();

        if (messageWindow != null)
        {
            messageWindow.gameObject.SetActive(true);
        }

        if (screenFader != null)
        {
            screenFader.gameObject.SetActive(true);
        }
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals,
                                          GameObject goalLayout, int spacingWidth)
    {
        if (goalLayout != null && collectionGoals != null && collectionGoals.Length != 0)
        {
            RectTransform rectXform = goalLayout.GetComponent<RectTransform>();

            rectXform.sizeDelta =
                new Vector2(collectionGoals.Length * spacingWidth, rectXform.sizeDelta.y);

            CollectionGoalPanel[] panels =
                goalLayout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < panels.Length; i++)
            {
                if (i < collectionGoals.Length && collectionGoals[i] != null)
                {
                    panels[i].gameObject.SetActive(true);
                    panels[i].collectionGoal = collectionGoals[i];
                    panels[i].SetupPanel();
                }
                else
                {
                    panels[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals) =>
        SetupCollectionGoalLayout(collectionGoals, collectionGoalLayout, collectionGoalBaseWidth);

    public void UpdateCollectionGoalLayout(GameObject goalLayout)
    {
        if (goalLayout != null)
        {
            CollectionGoalPanel[] panels = goalLayout.GetComponentsInChildren<CollectionGoalPanel>();

            if (panels != null && panels.Length != 0)
            {
                foreach (CollectionGoalPanel panel in panels)
                {
                    if (panel != null && panel.isActiveAndEnabled)
                    {
                        panel.UpdatePanel();
                    }
                }
            }
        }
    }

    public void UpdateCollectionGoalLayout() => UpdateCollectionGoalLayout(collectionGoalLayout);

    public void EnableTimer(bool state)
    {
        if (timer != null)
        {
            timer.gameObject.SetActive(state);
        }
    }

    public void EnableMovesCounter(bool state)
    {
        if (movesCounter != null)
        {
            movesCounter.SetActive(state);
        }
    }

    public void EnableColletionGoalLayout(bool state)
    {
        if (collectionGoalLayout != null)
        {
            collectionGoalLayout.gameObject.SetActive(state);
        }
    }
}
