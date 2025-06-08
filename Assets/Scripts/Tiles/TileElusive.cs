using UnityEngine;

public class TileElusive : Tile, ITileElusionArea

{
    [SerializeField] private ParticleSystem elusiveEffect;
    public override void SetShip(GameObject ship)
    {
        if (ship != null)
        {
            tileShip = ship;
            tileShip.transform.position = gameObject.transform.position;
            SetType(this._type, ship.GetComponent<AShip>().faction);
            ElusiveArea();
        }
    }

    public void ElusiveArea()
    {
        Debug.Log("Set dodgechance to 1 in tile: " + name);
        tileShip.GetComponent<AShip>().dodgeChance = 1;
    }
}