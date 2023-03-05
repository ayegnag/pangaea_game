#if UNITY_EDITOR
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(AdvancedRuleTiles))]
    [CanEditMultipleObjects]
    public class AdvancedRuleTileEditor : RuleTileEditor
    {
        public Texture2D AnyIcon;
        public Texture2D SpecifiedIcon;
        public Texture2D NothingIcon;

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {   
            if ( rect != null) {
                switch (neighbor)
                {
                    case AdvancedRuleTiles.Neighbor.Any:
                        GUI.DrawTexture(rect, AnyIcon);
                        return;
                    case AdvancedRuleTiles.Neighbor.Specified:
                        GUI.DrawTexture(rect, SpecifiedIcon);
                        return;
                    case AdvancedRuleTiles.Neighbor.Nothing:
                        GUI.DrawTexture(rect, NothingIcon);
                        return;
                }
            }

            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}
#endif