using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimpleTDMechanicsTests
{
    [Test]
    public void TowerDefense_RangeCalculation_Works()
    {
        // This test simulates how tower range detection works
        
        // Arrange - Set up positions and range
        Vector3 towerPosition = new Vector3(5, 0, 5);
        float towerRange = 3.5f;
        
        // Various enemy positions to test
        Vector3 enemyInRange = new Vector3(7, 0, 4);       // Inside range
        Vector3 enemyAtRangeEdge = new Vector3(8.5f, 0, 5); // At range edge
        Vector3 enemyOutOfRange = new Vector3(9, 0, 5);    // Outside range
        
        // Act - Calculate distances
        float distanceToInRange = Vector3.Distance(towerPosition, enemyInRange);
        float distanceToEdge = Vector3.Distance(towerPosition, enemyAtRangeEdge);
        float distanceToOutOfRange = Vector3.Distance(towerPosition, enemyOutOfRange);
        
        // Assert
        Assert.Less(distanceToInRange, towerRange, "Enemy should be in tower range");
        Assert.AreEqual(towerRange, distanceToEdge, 0.001f, "Enemy should be exactly at tower range");
        Assert.Greater(distanceToOutOfRange, towerRange, "Enemy should be outside tower range");
    }
    
    [Test]
    public void TowerDefense_DamageCalculation_Works()
    {
        // This test simulates damage calculation
        
        // Arrange
        float initialHealth = 100f;
        float towerDamage = 25f;
        
        // Simulate upgrading tower
        float upgradedDamage = towerDamage * 1.5f; // 50% damage increase
        
        // Act - Calculate remaining health after hits
        float healthAfterOneHit = initialHealth - towerDamage;
        float healthAfterTwoHits = healthAfterOneHit - towerDamage;
        float healthAfterUpgradedHit = initialHealth - upgradedDamage;
        
        // Assert
        Assert.AreEqual(75f, healthAfterOneHit, "Health after one hit should be reduced correctly");
        Assert.AreEqual(50f, healthAfterTwoHits, "Health after two hits should be reduced correctly");
        Assert.AreEqual(62.5f, healthAfterUpgradedHit, "Upgraded tower should deal increased damage");
    }
    
    [Test]
    public void TowerDefense_PathFollowing_MovesInCorrectDirection()
    {
        // This test simulates enemy path following
        
        // Arrange - Create path points
        List<Vector2> path = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(5, 0),
            new Vector2(5, 5),
            new Vector2(10, 5)
        };
        
        // Current position
        Vector2 currentPos = new Vector2(2, 0);
        int pathIndex = 1; // Moving toward point 1 (5,0)
        
        // Act - Calculate movement direction
        Vector2 targetPos = path[pathIndex];
        Vector2 moveDirection = (targetPos - currentPos).normalized;
        
        // Assert
        Assert.AreEqual(new Vector2(1, 0), moveDirection, "Enemy should move right toward next point");
    }
}