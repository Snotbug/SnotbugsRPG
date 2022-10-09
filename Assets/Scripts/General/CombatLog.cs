using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatLog : MonoBehaviour
{
    [field : SerializeField] public TMP_Text Log { get; private set; }

    private Queue<string> StringQueue = new Queue<string>();
    private bool Processing = false;

    public void Print(string s)
    {
        StringQueue.Enqueue(s);
        if(!Processing) { Process(); }
    }

    // public async void Process()
    // {
    //     if(StringQueue.Count <= 0 ) { return; }
    //     Log.text =  StringQueue.Dequeue();
    //     Processing = true;
    //     await Task.Delay(1000);
    //     Log.text = "";
    //     Processing = false;
    //     Process();
    // }

    public void Process()
    {
        if(StringQueue.Count <= 0 ) { return; }
        Log.text =  StringQueue.Dequeue();
        Processing = true;
        StartCoroutine(WaitABit(() =>
        {
            Log.text = "";
            Processing = false;
            Process();
        }));
    }

    public IEnumerator WaitABit(Action onReturn)
    {
        yield return new WaitForSeconds(1f);
        onReturn();
    }
}
