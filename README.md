# SimpleSync
Synchronize scene objects between your unity app and the editor through socket using adb forward.

**When developing for mobile VR, getting feedback for a small change can take several minutes.** I have tried to find some tools to help me and [unity-simple-sync](https://github.com/teadrinker/unity-simple-sync) is the only one that could help me here. And the shortage is clearly, **it needs a server to save the status of objects in a json file after you adding a particular component in every single object you want to sync in the scene.**

So I decide to do it myself.
 * **No need of a server**. I let the mobile APP start a server and bind the local port, and then let the editor to connect the mobile through adb server using "adb forward".
 * **Using FlatBuffers instead of JSON**. If you have a reason to not use JSON, then you should not use it :). In fact, ProtoBuffers is the one that I am familiar but dueing to the annoying incompatibility problem in early Unity Editor (before 5.5 Unity use .NET 2.0 Framework), I pick FB to have a try.
 * **DONOT need to rebuild APP** if you want to sync another object in scene. It will be useless if I have to rebuild the app when I need to sync another object, so I add a communication protocol to let editor to tell app which ones need to be synchronized and controlled.
 * **Only need **a line of script** or **a component** in your scene. You only need to initialize this tool using a single line code at start of your app or jsut draging a component to the first building scene.
 * **Sync objects from APP and vice verse**. This will be very useful for debugging.
 * **USB and WiFi connection supported**. I integrate some "adb commands" with the editor for easy use.

# Usage
Initialize your app by using code `SimpleSync.SyncManager.Init()` at the start of your APP, or drag component `SimpleSync.cs` in your first buiding scene. Then build your app and install it.

## In editor, open scene "SyncMain.unity" and select the gameobject in Hierarchy and you will see below image in Inspector:
 * ![1](https://cloud.githubusercontent.com/assets/18274604/25087676/35763b50-23a3-11e7-9a14-e04cec447447.png)
 * Sync Frame Rate is how many times to sync in one second. This could be changed in RUNNING!
 * You could start your APP if you fill "Android Activity" with the right one and click the button with your device connected correctly.
 * If you want to use through WiFi, check "Wireless Connection" checkbox and fill the proper IP Address of your device, then click "Connect Device" button will help you to run the adb command to connect your device through network.
 * Use "ADB devices" button to check out if the device connected to your PC correctly. You could find out from the console.

## Then you start this in editor (make sure the APP running on your device too), if everything goes well your Inspector will looks like this:
![2](https://cloud.githubusercontent.com/assets/18274604/25087661/1f5024ee-23a3-11e7-9235-4b421e97e799.png)
 * Click "Request Sycn Scene" and you will see it loads the same scene running in device.

## Once the scene loaded, a new window named "Simple Sync" will appear, and you could select some objects in Hierarchy and see the window:
![3](https://cloud.githubusercontent.com/assets/18274604/25087669/2d38af4a-23a3-11e7-9ed1-49e0205fb8fa.png)
 * "Request D2E Sync" button will let selected objects in editor "follows" the pattern in device app.
 * "Request E2D Control" button will do the vice verse, you could change the selected objects in editor and it would also affect in device app.
 
# Be attention, the name of gameobject in scene is used for app to find out which one to be synced, so duplicated names will cause problems.
 
# This is a simple tool for development so use at your own risk. Right now, only Position, Rotation and Scale could be synced, while it will be easy to extend.