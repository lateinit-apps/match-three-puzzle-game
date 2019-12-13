using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public RectTransform collectionGoalLayout;

    public int collectionGoalBaseWidth = 125;

    CollectionGoalPanel[] collectionGoalPanels;

    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals)
    {
        if (collectionGoalLayout != null && collectionGoals != null && collectionGoals.Length != 0)
        {
            collectionGoalLayout.sizeDelta =
                new Vector2(collectionGoals.Length * collectionGoalBaseWidth,
                            collectionGoalLayout.sizeDelta.y);

            collectionGoalPanels =
                collectionGoalLayout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < collectionGoalPanels.Length; i++)
            {
                if (i < collectionGoals.Length && collectionGoals[i] != null)
                {
                    collectionGoalPanels[i].gameObject.SetActive(true);
                    collectionGoalPanels[i].collectionGoal = collectionGoals[i];
                    collectionGoalPanels[i].SetupPanel();
                }
                else
                {
                    collectionGoalPanels[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateCollectionGoalLayout()
    {
        foreach (CollectionGoalPanel panel in collectionGoalPanels)
        {
            if (panel != null && panel.gameObject.activeInHierarchy)
            {
                panel.UpdatePanel();
            }
        }
    }
}
