using UnityEngine;
using UnityEditor;

public class PickGameViewSizeWindow : EditorWindow, IHasCustomMenu
{
    [MenuItem("界面工具/游戏视图分辨率")]
    public static void Init()
    {
        GetWindow<PickGameViewSizeWindow>();
    }

    public void Awake()
    {
        titleContent.text = "Pick View Size";
        minSize = new Vector2(150f,16f);
    }

    protected void OnGUI()
    {
        var currentGroup = GameViewSizes.instance.currentGroup;
        int count = currentGroup.GetTotalCount();
        float curWidth = 0;
        float curX = 0, curY = 0;
        for (int i = currentGroup.GetBuiltinCount(); i < count; i++)
        {
            var viewSize = currentGroup.GetGameViewSize(i);
            var content = EditorGUIUtility.TempContent(viewSize.displayText);
            var size = EditorStyles.toolbarButton.CalcSize(content);
            size.x = Mathf.Min(size.x, Mathf.Max(minSize.x, position.width));
            if (size.x + curWidth > position.width)
            {
                curX = 0;
                curY += size.y;
                curWidth = 0;
                i--;
                continue;
            }

            curWidth += size.x;
            Rect rect = new Rect(curX, curY, size.x, size.y);
            curX += size.x;
            if (GUI.Button(rect, viewSize.displayText, EditorStyles.toolbarButton))
            {
                GameView.GetMainGameView().SizeSelectionCallback(i, viewSize);
                GameView.GetMainGameView().Repaint();
            }
        }
    }

    public void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(EditorGUIUtility.TextContent("批量自动添加分辨率"), false, TestAddViewSize);
    }

    private void TestAddViewSize()
    {
        var currentGroup = GameViewSizes.instance.currentGroup;
        GameViewSize[] list =
        {
            new GameViewSize(GameViewSizeType.FixedResolution, 2340, 1080, "☆设计"),
            new GameViewSize(GameViewSizeType.FixedResolution, 1920, 1080, "♡标准"),
            new GameViewSize(GameViewSizeType.FixedResolution, 1024, 768, "♢平板"),
            new GameViewSize(GameViewSizeType.FixedResolution, 2532, 1170, "i12 Max"),
            new GameViewSize(GameViewSizeType.FixedResolution, 1334, 750, "i7"),
            new GameViewSize(GameViewSizeType.FixedResolution, 960, 640, "i4"),
        };
        foreach (var item in list)
        {
            int count = currentGroup.GetTotalCount();
            for (int i = currentGroup.GetBuiltinCount(); i < count; i++)
            {
                var viewSize = currentGroup.GetGameViewSize(i);
                if (viewSize.width == item.width && viewSize.height == item.height)
                {
                    break;
                }

                if (i == count - 1)
                {
                    currentGroup.AddCustomSize(item);
                    GameViewSizes.instance.SaveToHDD();
                }
            }
        }
    }
}

