using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
// #TODO: Split this up, we're freaking OOP, not Procedural...
// I'm super tired, I don't know what the I'm thinking right now....
public interface ICharacter : IEntity, IRenderer {
    /// <summary>
    /// An offensive item that allows the character to attack.
    /// </summary>
    /// <remarks>The <see cref="Weapon"/> can be null, effectively using hands or no attack</remarks>
    IWeapon Weapon { get; }

    /// <summary>
    /// A defensive item that protects the character
    /// </summary>
    /// <remarks>The <see cref="Defense"/> can be null, effectively leaving the character defenseless</remarks>
    IDefensive Defense { get; }

    /// <summary>
    /// Check whether the character can swim
    /// </summary>
    bool CanSwim { get; }

    /// <summary>
    /// Check whether the character can attack
    /// </summary>
    bool CanAttack { get; }

    /// <summary>
    /// Check whether the character can defend
    /// </summary>
    bool CanDefend { get; }

    /// <summary>
    /// Check whether the character can climb
    /// </summary>
    bool CanClimb { get; }

    /// <summary>
    /// Check whether the character can move
    /// </summary>
    bool CanMove { get; }

    /// <summary>
    /// Check whether the character can jump
    /// </summary>
    bool CanJump { get; }

    /// <summary>
    /// Check whether the character is invincible
    /// </summary>
    bool IsInvincible { get; }

    /// <summary>
    /// The Sprite of the entity
    /// </summary>
    AnimatedSprite32 Sprite { get; set; }

    /// <summary>
    /// Check whether the character is over water
    /// </summary>
    /// <returns></returns>
    bool IsSwimming();

    /// <summary>
    /// Check whether the character is near a climbable wall
    /// </summary>
    /// <returns></returns>
    bool IsClimbing();

    /// <summary>
    /// Die, simple as that.
    /// </summary>
    void Die();

    /// <summary>
    /// Jump up in the air
    /// </summary>
    void Jump();

    /// <summary>
    ///  Duck and Cover!
    /// </summary>
    void Duck();

    /// <summary>
    /// Attack with the specified loaded weapon. NULL Weapons should just return immediately
    /// </summary>
    void Attack();

    /// <summary>
    /// Defend with the specified loaded defense. NULL Defense should just return immediately.
    /// </summary>
    void Defend();

    /// <summary>
    /// Based on a consumable, A heal can actually take damage.
    /// </summary>
    /// <param name="hp">The amount of health points to restore to the character</param>
    void Heal(int hp);

    /// <summary>
    /// Regardless of the number, taking damage should always take damage. The number should be absolute.
    /// </summary>
    /// <param name="damage">An absolute value of health points to subtract from the character</param>
    void TakeDamage(int damage);
}