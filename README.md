# Unity package template
This template provides solution on how to create and maintain your package, while allowing it to be distributed through GitHub.
It has CI actions, which creates/updates branch called `upm` after each commit to `main`.
This is very usefull because:
1. you can have whole project(with all settings) in repo so cotributing is really simple. 
2. users don't need to worry about extracting required folders from repo and able to just install only `upm` branch through Package Manager.

Look at [upm](https://github.com/STARasGAMES/Unity-package-repo-setup-template/tree/upm) branch to see how it looks.

Source of template: https://medium.com/openupm/how-to-maintain-upm-package-part-1-7b4daf88d4c4

## Steps to setup your package repo

* Create new repo using this project as template.
* Rename folder "Packages/PACKAGE_NAME" to represent your package name and namespace (if you are using Rider). For ex: `Packages/SaG.Dependencies`.
* Change PACKAGE_NAME in ".github/workflows/ci.yml" to your packages's folder name.
(IMPORTANT: it's easier to use github web client to do this step, otherwise there is a big chance to mess around with git credentials)
* Copy your package name, because you will need to paste it several time.
  1. In Unity go to your package folder and select package.json file. Change all appropriate fields.
  2. In Unity go to your package folder and change names for all `.asmdef` files.
* Change `README.md` and `Packages/PACKAGE_NAME/README.md` installation sections accordingly to your github repo.
8 Remove from `README.md` unnecessary text .

## Installation
Install via git url by adding this entry in your **manifest.json**

`"com.starasgames.tmpro-dynamic-data-cleaner": "https://github.com/STARasGAMES/tmpro-dynamic-data-cleaner.git#upm"`
