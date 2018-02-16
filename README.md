# Microsoft Eye Drive

Microsoft Eye Drive is a virtual joystick that you control using only your eyes.

Eye Drive started in 2017 as a collaboration between [Team Gleason](http://www.teamgleason.org/) and Microsoft, and was kicked off during Microsoft's annual [OneWeek Hackathon](https://blogs.microsoft.com/firehose/2017/07/24/microsofts-one-week-hackathon-kicks-off-this-year-with-nonprofits-hacking-alongside-employees/).  Originally known as ["Pilot 37"](https://github.com/TeamGleason/Pilot37), our early goal was to enable [Steve Gleason](https://twitter.com/TeamGleason) (former New Orleans Saints football player who played under the number 37 and who today lives with ALS), to fly a drone.  However, we pretty quickly decided that three pounds of flying propellers being (sort of) controlled by hacked up prototype software _might not_ be the best idea we ever had, so we switched to driving around a remote controlled car onto which we mounted a drone's FPV (first person view) camera.  For more information on this project, see this [Github Repository](https://github.com/TeamGleason/Pilot37).

[![Steve Driving A Car](https://github.com/Microsoft/EyeDrive/raw/master/Media/SteveDriving.jpg)](https://github.com/Microsoft/EyeDrive/raw/master/Media/SteveDriving.mp4)

## Frequently Asked Questions

1.  This is cool!  But why does the application here look different than the application in the photo?

	The application in the photo is from Pilot 37 which was a quick prototype developed during a hackathon.  However, it soon became clear that a joystick that you control with your eyes has a lot more applications than just driving around an RC car, so we separated out that part of the work and spent more time designing and polishing it.  We discovered that a smaller set of buttons was easier for people to understand; because we wanted to make the app accessible to a wide audience with little training time, we simplified.

2.  Great, but the app doesn't do anything?  Why ship just a joystick app?

	Alright.  Good point.  Push the buttons, nothing happens.  Silly app, right?

	Well, the point here is twofold.  First, most people haven't ever seen an eye control sensor and don't know what it can do.  Let's introduce that to them with a simple app that has lots of possibilities.

	Second, sure, any joystick alone isn't particularly useful.  But people attach joysticks to things in useful ways all the time.  Xbox consoles.  Fighter jets.  Robot arms.  So here you go, here's a new type of joystick that doesn't require hands.  Go attach it to a rocket ship or something.

	Anyone is free to experiment with Eye Drive to build whatever useful things they might imagine; what will you build?

3.  Hey, you guys announced some cool new [Windows Eye Control](https://support.microsoft.com/en-us/help/4043921/windows-10-get-started-eye-control) features in the last Windows release, yeah?  Does this have anything to do with them?

	Awesome!  Thanks for noticing!  We worked hard on that.  Not yet.  This app is based on older 'Windows Desktop' technology because that's what is available to use if you write an eye control app today.  However, we're working on updates, Windows is getting better all the time, and we just worked to create a [new USB Standard for eye tracking cameras](http://www.usb.org/developers/hidpage/HUTRR74_-_Usage_Page_for_Head_and_Eye_Trackers.pdf), so it's a pretty safe bet that more good things are coming!  Keep an eye out for events like [Microsoft Build 2018](https://www.microsoft.com/en-us/build) to learn more if you're into this stuff and you want to be part of an exciting future...

4.  What are your future plans for this project?

	We'll keep working on this to improve it, but it's a pretty straightforward app so there's not a lot we want to add right now.  Simple is good.  We'll keep it up to date as Windows Eye Control changes and improves over time.

	Mostly we want to see what interesting things you'll do with this.  We created an easy to implement interface that you can implement with whatever you think needs to be driven around.  Let's turn this question around, what are *your* plans for this project?

5.  OK, I downloaded the app and compiled it, what do I need on my laptop or Surface to get it to work?

    I'd suggest starting with a [Tobii Eye Tracker 4C](https://www.amazon.com/Tobii-Eye-Tracker-4C-PC).  It works great with this app.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Privacy & Cookies

https://go.microsoft.com/fwlink/?LinkId=521839
