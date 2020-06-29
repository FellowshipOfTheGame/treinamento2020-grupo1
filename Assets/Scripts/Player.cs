﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    // Components
    Animator playerAnimator;
    Rigidbody2D playerRigidbody2D;
    CharacterAnimation characterAnimation;

    public HealthBar healthBar;
    public ManaBar manaBar;

    [Space]
    [Header("Equipment")]
    public Weapon weapon;
    public Armor armor;
    public ClassItem classItem;

    // timers n stuff
    protected float attackBuffer;

    new void Start() {
        base.Start();

        playerAnimator = gameObject.GetComponent<Animator>();
        playerRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        characterAnimation = gameObject.GetComponent<CharacterAnimation>();
    }

    new void Update() {
        base.Update();

        HandleMovement();
        HandleAttack();

        healthBar.SetHealth(currentHealth / maximumHealth);
        manaBar.SetMana(currentMana / maximumMana);

        if (armor != null)
            characterAnimation.armorSheet = armor.appearance;
    }

    void HandleAttack() {
        Vector2 mouseWolrdPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDirection = new Vector2(mouseWolrdPos.x - transform.position.x, mouseWolrdPos.y - transform.position.y).normalized;

        if (Input.GetButton("Fire1") && attackBuffer == 0f && weapon != null) {
            weapon.Attack(transform, mouseDirection.normalized, new ContactFilter2D() { useLayerMask = true, layerMask = 1 << 9 }, (classItem ? classItem.name.Equals("grimoire") : false));
            attackBuffer = GetAttackSpeed();
        }

        if (Input.GetButton("Fire2") && attackBuffer == 0f  && classItem != null) {
            classItem.TriggerSkill(weapon.weaponType, transform, mouseDirection.normalized, new ContactFilter2D() { useLayerMask = true, layerMask = 1 << 9 });
            attackBuffer = GetAttackSpeed();
        }

        if (attackBuffer > 0f)
            attackBuffer = Mathf.Clamp(attackBuffer - Time.deltaTime, 0f, attackBuffer);
    }

    void HandleMovement() {
        float dirHorizontal = Input.GetAxisRaw("Horizontal");
        float dirVertical = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(dirHorizontal, dirVertical).normalized;

        playerAnimator.SetFloat("horizontalVelocity", dirHorizontal);
        playerAnimator.SetFloat("verticalVelocity", dirVertical);

        playerRigidbody2D.velocity = direction * GetMoveSpeed();
    }
}