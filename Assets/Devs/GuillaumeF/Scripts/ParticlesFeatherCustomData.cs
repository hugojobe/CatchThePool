using UnityEngine;

public class ParticlesFeatherCustomData : MonoBehaviour
{
    private ParticleSystem pS;
    private ParticleSystemRenderer particleRenderer;
    private MaterialPropertyBlock propertyBlock;

    void Start()
    {
        
        pS = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        propertyBlock = new MaterialPropertyBlock();

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pS.main.maxParticles];
        var psCD = pS.customData;
        int particleCount = pS.GetParticles(particles);

            //float customValue = particles[i].customData1.x;

            //propertyBlock.SetFloat("_FeatherColorValue", psCD);

            particleRenderer.SetPropertyBlock(propertyBlock);
        
    }
}
