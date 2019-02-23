using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 功能:在编辑器面板上直观修改挂接Player类脚本中的相关数据
/// </summary>

[CustomEditor(typeof(Player))]
//必须要让该类继承自Editor,且不需要导入UnityEditor程序集
public class PlayerInspector : Editor
{

    Player player;
    bool showWeapons;

    void OnEnable()
    {
        //获取当前编辑自定义Inspector的对象
        player = (Player)target;
    }

    //执行这一个函数来一个自定义检视面板
    public override void OnInspectorGUI()
    {
        //设置整个界面是以垂直方向来布局
        EditorGUILayout.BeginVertical();

        //空两行
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //绘制palyer的基本信息
        EditorGUILayout.LabelField("Base Info");
        player.id = EditorGUILayout.IntField("Player ID", player.id);
        player.playerName = EditorGUILayout.TextField("PlayerName", player.playerName);

        //空三行
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //绘制Player的背景故事
        EditorGUILayout.LabelField("Back Story");
        player.backStory = EditorGUILayout.TextArea(player.backStory, GUILayout.MinHeight(100));

        //空三行
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //使用滑块绘制 Player 生命值
        player.health = EditorGUILayout.Slider("Health", player.health, 0, 100);

        //根据生命值设置生命条的背景颜色
        if (player.health < 20)
        {
            GUI.color = Color.red;
        }
        else if (player.health > 80)
        {
            GUI.color = Color.green;
        }
        else
        {
            GUI.color = Color.gray;
        }

        //指定生命值的宽高
        Rect progressRect = GUILayoutUtility.GetRect(50, 50);

        //绘制生命条
        EditorGUI.ProgressBar(progressRect, player.health / 100.0f, "Health");

        //用此处理，以防上面的颜色变化会影响到下面的颜色变化
        GUI.color = Color.white;

        //空三行
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //使用滑块绘制伤害值
        player.damage = EditorGUILayout.Slider("Damage", player.damage, 0, 20);

        //根据伤害值的大小设置显示的类型和提示语
        if (player.damage < 10)
        {
            EditorGUILayout.HelpBox("伤害太低了吧！！", MessageType.Error);
        }
        else if (player.damage > 15)
        {
            EditorGUILayout.HelpBox("伤害有点高啊！！", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("伤害适中！！", MessageType.Info);
        }

        //空三行
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //设置内容折叠
        showWeapons = EditorGUILayout.Foldout(showWeapons, "Weapons");
        if (showWeapons)
        {
            player.weaponDamage1 = EditorGUILayout.FloatField("Weapon 1 Damage", player.weaponDamage1);
            player.weaponDamage2 = EditorGUILayout.FloatField("Weapon 2 Damage", player.weaponDamage2);
        }

        //空三行
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //绘制鞋子信息
        EditorGUILayout.LabelField("Shoe");
        //以水平方向绘制
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(50));
        player.shoeName = EditorGUILayout.TextField(player.shoeName);
        EditorGUILayout.LabelField("Size", GUILayout.MaxWidth(50));
        player.shoeSize = EditorGUILayout.IntField(player.shoeSize);
        EditorGUILayout.LabelField("Type", GUILayout.MaxWidth(50));
        player.shoeType = EditorGUILayout.TextField(player.shoeType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}