/// <summary>Qualquer coisa que aceita dano (obstáculos, chefes, projéteis destrutíveis).</summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}
