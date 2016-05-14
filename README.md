# [deprecated] Modern-Gitter [![Join the chat at https://gitter.im/Odonno/Modern-Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Odonno/Modern-Gitter?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## The project is now a Windows 10 app, source code available here https://github.com/Odonno/modern-gitter-winjs

[![Build status](https://ci.appveyor.com/api/projects/status/xo8h2dbppqvtn162?svg=true)](https://ci.appveyor.com/project/Odonno/modern-gitter)

A Gitter client application for Windows 8.1 &amp; Windows Phone 8.1

<center>
<img src="/images/modern-gitter-home.png"  height="400" />
<img src="/images/modern-gitter-room.png"  height="400" />
</center>

## Features

Modern Gitter contains a small set of features that directly target Gitter API. Here is the key features :

* Retrieve your current rooms
* Search among your current rooms
* Chat in realtime (see & send messages)
* Use microphone to send messages
* Receive realtime notifications (in-app notifications)
* Receive delayed notifications (toasts notifications : unread items and mentions)

## Frameworks

This project makes use of several frameworks like : 

* [MVVMLight](http://www.mvvmlight.net/)
* [Rx (.NET)](https://rx.codeplex.com/)
* [JSON.NET](http://www.newtonsoft.com/json)
* [HtmlAgilityPack](https://htmlagilitypack.codeplex.com/)
* [Microsoft Application Insights](https://github.com/Microsoft/ApplicationInsights-dotnet)
* [Gitter#](https://github.com/Odonno/gitter-api-pcl)
* [Gitter# Auth](https://github.com/Odonno/gitter-api-auth)

## Contribute

You can contribute to this project. There is several rules to follow :

* Use the angular git commit convention
* Create Pull Request with an understandable title and a short message that explains your contribution
* Be free to innovate !

### Commit convention

We follow the angular git commit convention (https://github.com/angular/angular.js/blob/master/CONTRIBUTING.md#-git-commit-guidelines).
Here are some examples that can help you to visualise how to make meaningful commits.

When fixing a bug with the notifications

    fix(notifications): remove call that push twice the same notification

When adding a new feature related to notifications

    feat(notifications): add push notifications when someone mentions user

When fixing an issue with the build / improving the build system

    chore(app): improve build system

When adding some documentation

    docs(readme): add a contribute section

### List of contributors

I want to thank the following contributors who have helped me with this project.

* [gep13](https://github.com/gep13)
* [bobmulder](https://github.com/bobmulder)
* [wassim-azirar](https://github.com/wassim-azirar)
* [NPadrutt](https://github.com/NPadrutt)
* [corentinMiq](https://github.com/corentinMiq)
