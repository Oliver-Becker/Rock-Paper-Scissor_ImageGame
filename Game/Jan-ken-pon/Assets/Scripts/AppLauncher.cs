using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
 
public class AppLauncher : MonoBehaviour {
	Process process = null;
	StreamWriter messageStream;

    public void StartProcess(string imgFilePath) {
		string pythonFilePath = Application.dataPath + "/NeuralNetwork_Python/testedoprograma.py ";
        try {
            process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = pythonFilePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += new DataReceivedEventHandler( DataReceived );
            process.ErrorDataReceived += new DataReceivedEventHandler( ErrorReceived );
            process.Start();
            process.BeginOutputReadLine();
            messageStream = process.StandardInput;

			print("Finished calling the process!");
			print("messageStream = " + messageStream);
       
            UnityEngine.Debug.Log( "Successfully launched app" );
        } catch( Exception e ) {
            UnityEngine.Debug.LogError( "Unable to launch app: " + e.Message );
        }
    }
 
 
    void DataReceived( object sender, DataReceivedEventArgs eventArgs ) {
		print("Received data! Lets try to print it");
		print("sender = " + sender);
		print("eventArgs = " + eventArgs);
        // Handle it
    }
 
 
    void ErrorReceived( object sender, DataReceivedEventArgs eventArgs ) {
        UnityEngine.Debug.LogError( eventArgs.Data );
    }
 
 
    void OnApplicationQuit() {
        if( process != null  && !process.HasExited ) {
            process.Kill();
        }
    }
}