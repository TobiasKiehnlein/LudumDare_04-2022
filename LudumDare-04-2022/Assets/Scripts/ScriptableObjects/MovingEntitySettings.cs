using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "MovingEntitySettings", menuName = "CustomGameSettings/MovingEntitySettings",
        order = 0)]
    public class MovingEntitySettings : ScriptableObject
    {
        // General
        [SerializeField] public int maxHitcount = 5;
        [SerializeField] public float minMoveDistance = 0.0f; // Per update and iteration
        [SerializeField] public float maxMoveDistance = 10.000f; // Per update and iteration
        [SerializeField] public float wallCollisionDistance = 0.2f;
        [SerializeField] public float stateMultiplierReductionSpeed = 0.1f;
        [SerializeField] public float stateUpdateCooldown = 3f;
        [SerializeField] public float wallHitDirectionCooldown = 0.25f;
        [SerializeField] public float steeringMultiplier = 0.1f;

        [SerializeField] public float debugDrawDuration = 5f;

        // Types
        /*
            Death,
            Human,
            Cross,
            Totem,
            Obstacle,
            VisualOnly,
        */

        // Human
        /*
            Chilling,
            Crowdy,
            AntiCrowdy,
            Jogging,
            Afraid,
        */
        // ReSharper disable InconsistentNaming
        
        // TargetSpeed
        [SerializeField] public float human_chilling_targetSpeed = 0;
        [SerializeField] public float human_crowdy_targetSpeed = 2;
        [SerializeField] public float human_antiCrowdy_targetSpeed = 4;
        [SerializeField] public float human_jogging_targetSpeed = 10;
        [SerializeField] public float human_afraid_targetSpeed = 15;

        // Wall speed influence
        [SerializeField] public float human_chilling_wallSpeedInfluence = 0;
        [SerializeField] public float human_crowdy_wallSpeedInfluence = 0;
        [SerializeField] public float human_antiCrowdy_wallSpeedInfluence = +5;
        [SerializeField] public float human_jogging_wallSpeedInfluence = -5;
        [SerializeField] public float human_afraid_wallSpeedInfluence = -15;
        
        // Steering strength base
        [SerializeField] public float human_death_steeringStrengthBase = 0.7f;
        [SerializeField] public float human_human_steeringStrengthBase = 0.5f;
        [SerializeField] public float human_human_steeringStrengthCollision = 1;
        [SerializeField] public float human_deadHuman_steeringStrengthBase = 0.5f;
        [SerializeField] public float human_cross_steeringStrengthBase = 0.5f;
        [SerializeField] public float human_totem_steeringStrengthBase = 0.3f;
        [SerializeField] public float human_totem_steeringStrengthCollision = 2;
        [SerializeField] public float human_obstacle_steeringStrengthBase = 0.2f;
        [SerializeField] public float human_visualOnly_steeringStrengthBase = 0.1f;
        [SerializeField] public float human_visualOnly_steeringStrengthCollision = 0.7f;

        // Steering strength mult
        [SerializeField] public float human_chilling_steeringStrengthMult = 0;
        [SerializeField] public float human_crowdy_steeringStrengthMult = 1.5f;
        [SerializeField] public float human_antiCrowdy_steeringStrengthMult = 1.2f;
        [SerializeField] public float human_jogging_steeringStrengthMult = 0.1f;
        [SerializeField] public float human_afraid_steeringStrengthMult = 0.01f;

        // 
        
        // Death
        // ReSharper disable InconsistentNaming
        //[SerializeField] public float death_wallSpeedInfluence = +5;
    }
}