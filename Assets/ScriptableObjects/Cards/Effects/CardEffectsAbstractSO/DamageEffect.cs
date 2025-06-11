using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "ChangeClassEffect", menuName = "Scriptable Objects/Card Effects/Damage Effect")]
public class DamageEffect : AbstractEffectSO
{
    [SerializeField] public int damage;
    [SerializeField] public ParticleSystem incomingFire;
    
    [Header("Animation Settings")]
    [SerializeField] private float heightOffset = 100f;  // Altezza iniziale sopra la nave
    [SerializeField] private float animationDuration = 2.0f;  // Durata dell'animazione di discesa
    [SerializeField] private float particleLifetime = 1.0f;  // Quanto a lungo il sistema di particelle rimane attivo
    [SerializeField] private Ease easeType = Ease.InQuad;  // Il tipo di animazione (accelerazione)
    
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<AShip>(out AShip shipScript))
        {
            // Posizione della nave
            Vector3 shipPosition = effectStruct.obj.transform.position;
            
            // Posizione iniziale del sistema di particelle (alto sopra la nave)
            Vector3 startPosition = shipPosition + Vector3.up * heightOffset;
            
            // Istanzia il sistema di particelle nella posizione iniziale
            ParticleSystem particleInstance = Instantiate(incomingFire, startPosition, Quaternion.identity);
            
            // Attiva subito il sistema di particelle
            particleInstance.Play();

            // Termina l'effetto della carta
            EndEffect(0);
            
            // Inizia l'animazione di discesa
            particleInstance.transform.DOMove(shipPosition, animationDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    // Al momento del contatto...


                    // Istanzia un effetto di esplosione/impatto nella posizione finale (opzionale)
                    // CreateImpactEffect(shipPosition);

                    // Distruggi immediatamente il sistema di particelle
                    Destroy(particleInstance.gameObject);

                     // Infliggi il danno alla nave
                    shipScript.TakeDamage(damage);
                    
                });
        }
        else
        {
            // In caso non ci sia una nave, termina subito l'effetto
            EndEffect(0);
        }
    }
}