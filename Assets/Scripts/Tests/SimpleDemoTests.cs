using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimpleDemoTests
{
    [Test]
    public void BasicMathTest()
    {
        // Simple test that always passes
        Assert.AreEqual(4, 2 + 2);
    }
    
    [Test]
    public void VectorTest()
    {
        // Test basic Unity functionality
        Vector3 v1 = new Vector3(1, 0, 0);
        Vector3 v2 = new Vector3(0, 1, 0);
        Vector3 result = v1 + v2;
        
        Assert.AreEqual(new Vector3(1, 1, 0), result);
    }
    
    [UnityTest]
    public IEnumerator TimePassesTest()
    {
        // Test that demonstrates a frame-based test
        float startTime = Time.time;
        
        // Wait 3 frames
        yield return null;
        yield return null;
        yield return null;
        
        // Time should have advanced
        Assert.Greater(Time.time, startTime);
    }
}