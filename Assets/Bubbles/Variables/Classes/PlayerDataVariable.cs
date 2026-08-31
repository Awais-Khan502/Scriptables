
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/PlayerDataVariable")]
public class PlayerDataVariable : DataVariable<PlayerData>
{
        public override void SaveValue()
    {
        base.SaveValue();
    }

    public override void LoadValue()
    {
        base.LoadValue();
    }


}