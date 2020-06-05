@echo off

rem Use following command to create submodule 
rem git submodule add -b master https://github.com/Allors/allors2.git allors

rem Use following command to update all submodules
rem git submodule update --init --recursive

cd allors
git checkout master
git pull
cd ..
git add allors

@pause



