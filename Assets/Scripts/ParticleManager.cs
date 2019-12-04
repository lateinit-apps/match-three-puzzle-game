using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject breakFXPrefab;
    public GameObject clearFXPrefab;
    public GameObject doubleBreakFXPrefab;

    public GameObject bombFXPrefab;

    public void ClearPieceFXAt(int x, int y, int z = 0)
    {
        if (clearFXPrefab != null)
        {
            GameObject clearFX =
                Instantiate<GameObject>(clearFXPrefab, new Vector3(x, y, z), Quaternion.identity);

            ParticlePlayer particlePlayer = clearFX.GetComponent<ParticlePlayer>();

            if (particlePlayer != null)
            {
                particlePlayer.Play();
            }
        }
    }

    public void BreakTileFXAt(int breakableValue, int x, int y, int z = 0)
    {
        GameObject breakFX = null;
        ParticlePlayer particlePlayer = null;

        if (breakableValue > 1)
        {
            if (doubleBreakFXPrefab != null)
            {
                breakFX = Instantiate<GameObject>(doubleBreakFXPrefab,
                                                  new Vector3(x, y, z), Quaternion.identity);
            }
        }
        else
        {
            if (breakFXPrefab != null)
            {
                breakFX = Instantiate<GameObject>(breakFXPrefab,
                                                  new Vector3(x, y, z), Quaternion.identity);
            }
        }

        if (breakFX != null)
        {
            particlePlayer = breakFX.GetComponent<ParticlePlayer>();

            if (particlePlayer != null)
            {
                particlePlayer.Play();
            }
        }
    }

    public void BombFXAt(int x, int y, int z = 0)
    {
        if (bombFXPrefab != null)
        {
            GameObject bombFX =
                Instantiate<GameObject>(bombFXPrefab, new Vector3(x, y, z), Quaternion.identity);
            
            ParticlePlayer particlePlayer = bombFX.GetComponent<ParticlePlayer>();

            if (particlePlayer != null)
            {
                particlePlayer.Play();
            }
        }
    }
}
