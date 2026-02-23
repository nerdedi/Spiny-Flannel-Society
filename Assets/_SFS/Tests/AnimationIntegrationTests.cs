using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SFS.Animation;
using SFS.Core;

public class AnimationIntegrationTests
{
    GameObject bridgeObj;
    TranslationVerbBridge bridge;
    DefaultsRegistry registry;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // create a registry singleton
        var regGo = new GameObject("DefaultsRegistry");
        registry = regGo.AddComponent<DefaultsRegistry>();
        // manual Awake/Start called automatically by Unity next frame

        // create the bridge object
        bridgeObj = new GameObject("VerbBridge");
        bridge = bridgeObj.AddComponent<TranslationVerbBridge>();
        bridgeObj.AddComponent<CharacterAnimationDriver>();

        yield return null; // allow registry Awake to run
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.Destroy(registry.gameObject);
        Object.Destroy(bridgeObj);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ReadDefault_ShowsOverlay()
    {
        // ensure registry contains at least one default
        Assert.IsFalse(registry.AllRewritten());
        string key = registry.ListReadable()[0].Key;

        LogAssert.Expect(LogType.Log, "[UI] Default overlay:");

        bridge.BeginRead(key);
        // simulate the animation event firing on the same frame
        bridge.OnReadReveal();

        yield return null;
    }

    [UnityTest]
    public IEnumerator RewriteDefault_CommitsAndRecordsWindprint()
    {
        string key = registry.ListReadable()[0].Key;

        bridge.BeginRead(key);
        bridge.OnReadReveal();

        float before = registry.Progress;
        bridge.BeginRewriteCushion(key);
        bridge.OnRewriteCommit();

        Assert.Greater(registry.Progress, before);
        yield return null;
    }
}
