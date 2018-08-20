
<img src="DicsordBot/res/speaker.png" alt="drawing" style="width:100px;"/>


# Welcome to TDS </br> 
## **T**he **D**iscord **S**oundboard 2.X

</br></br>

Hi, 
this is a small Discord-Soundboard for streaming Sounds/Music into Discord - voice channels.
<br>
<br>
In contrast to a Soundbot, this application hosts a bot on your local machine for the duration of the playback.
This brings the advantage of an guaranteed availability of the service, combined with the abillity to stream local files from your Harddrive.
<br>

>Developer Information: <br/>
>All source code in the folders 'Bot' and 'Data' is able to run as standalone (per folder).
>The source code in 'Handle' requires both, 'Bot' and 'Data' -source to run unmodified.
>
>For this reason, a code documentation was added for those sectors

---

##### Planned Features (long term):

1. Something similiar to a playlist mode
2. Search mode, wich is providing titles via search on your disk for temporary use
3. Hotkeys to trigger sounds
3. Directly streaming from web plattforms (only if compatible with the law)

---

### Instructions:


##### What you need:
<br/>

1. A confirmed Discord Account (one time)
2. Rights to invite a bot to your server (one time)
3. Internet Connection (obviously)
4. Some soundfiles to stream

<br/>

##### What the bot needs:

1. Rights to join a channel (permanent)
2. Rights to transmit audio into that channel (permanent)
3. Rights to view all channels (comfort feature)
</br>

---
#### How to create the Bot:

If you don't already know the procedure, check out [this](doc/BotCreation.md) instruction ([doc/BotCreation.md](doc/BotCreation.md))

---

#### How to 'install' the Soundboard

We've now got a new installer for this application.

You only need to download and execute the install wizard.
Follow all steps and instruction and everything will be set up correctly.


If you compile the project by yourself, the opus and libsodium dll's won't get generated, so you'll need to download them seperately.

---

#### How to use the Bot

>##### In the future, there's an short introduction sequence planned, which explains the following steps on the first programm startup.
<br>

Before you can start to stream a file, you need to specify the ```token```, gathered in the first step when creating the bot.

For this you need to enter the settings Menu and enter the value in the corresponding field

![Enter your token](doc/enterToken.png "Enter Token") 


When entered the username and your token you can go ahead and edit the buttons on your desire.
Multiple button presses will add it to a queue.

When no username is specified, you will have to select a channel on the top right corner, by clicking on the circle.
![Set Buttons and Channel](doc/ButtonChannelSettings.png "Button Settings")





<br>
<br>


To start streaming a file, you simply need to edit one button, add your file with the filepicker, or by entering the path.
The next step is pressing the button, and the bot will join your channel (as long as you entered a valid username).

<!--insert image -->


---

> ###### F: Why can't I stream Videos from Youtube? <br>
>    1. Downloading videos can be illegal in some cases. Further, the separation of sound and video is not allowed. (As we know so far) <br>
>    2. This bot is built to fill the gap for streaming files from your disk. There're plenty of very good music bots out there to play online-videos. So there's no need, to use your own bandwith to stream a youtube video. <br/>
>    <br/>
>    But let's see what the future brings












