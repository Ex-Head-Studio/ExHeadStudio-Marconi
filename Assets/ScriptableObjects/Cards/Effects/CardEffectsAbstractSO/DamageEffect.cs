using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Scriptable Objects/Card Effects/Damage Effect")]
public class DamageEffect : AbstractEffectSO
{
    [SerializeField] public int damage;
    
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj == null)
        {
            Debug.LogError("Nessun oggetto specificato per l'effetto di danno");
            EndEffect(1);
            return;
        }
        
        // Applica danno diretto a una nave nemica
        if (effectStruct.obj.TryGetComponent<AShip>(out AShip shipScript) && 
            shipScript.faction == (int)Tile.Entity.enemy)
        {
            ApplyDamageToShip(shipScript);
        }
        // Attiva targeting se usato da nave alleata
        else if (effectStruct.obj.TryGetComponent<AllyShip>(out _))
        {
            ActivateTargeting();
        }
        // Verifica se è una tile con una nave
        else if (effectStruct.obj.TryGetComponent<Tile>(out Tile tile))
        {
            HandleTileEffect(tile);
        }
        else
        {
            Debug.LogError("L'oggetto non è una nave o una tile: " + effectStruct.obj.name);
            EndEffect(1);
        }
    }
    
    private void ApplyDamageToShip(AShip ship)
    {
        Debug.Log($"Applicando {damage} danno a {ship.shipName}");
        ship.TakeDamage(damage);
        EndEffect(0);
    }
    
    private void ActivateTargeting()
    {
        Debug.Log("Carta usata da nave alleata, attivo il targeting");
        GridManager.Instance.SetCurrentEffect(this);
        GridManager.Instance.EnableTileSelection(TileType.Enemy);
    }
    
    private void HandleTileEffect(Tile tile)
    {
        GameObject shipObject = tile.GetShip();
        if (shipObject != null && shipObject.TryGetComponent<AShip>(out AShip tileShip))
        {
            Debug.Log($"Applicando {damage} danno alla nave sulla tile: {tileShip.shipName}");
            tileShip.TakeDamage(damage);
            EndEffect(0);
        }
        else
        {
            Debug.LogWarning("La tile selezionata non contiene una nave");
            EndEffect(1);
        }
    }
}