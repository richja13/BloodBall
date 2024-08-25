using UnityEngine;

public class Ripple : MonoBehaviour
{
  public Material RippleMaterial;
    public float MaxAmount = 50f;
    public static bool RippleTrue;

    [Range(0, 1)]
    float Friction = 10f;

    float Amount = 10f;

    void Update()
    {
        if (Input.GetKey(KeyCode.O))
            RippleEffect();
    }

    void RippleEffect()
    {
        this.Amount = this.MaxAmount;
        Vector2 pos = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        this.RippleMaterial.SetFloat("_CenterX", pos.x);
        this.RippleMaterial.SetFloat("_CenterY", pos.y);
        this.RippleMaterial.SetFloat("_Amount", this.Amount);
        this.Amount *= this.Friction;
        RippleTrue = true;

        this.RippleMaterial.SetFloat("_Amount", this.Amount);
        this.Amount *= this.Friction;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, this.RippleMaterial);
    }
}
