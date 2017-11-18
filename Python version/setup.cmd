@echo off
cd %~dp0
python ez_setup.py --insecure
python get-pip.py
pip install pyglet
pause