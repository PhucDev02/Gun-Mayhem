## Python 3.10.9 
# Cài đặt môi trường python: 
- mở cmd tại thư mục game
- python -m venv venv
- .\venv\Scripts\activate
- pip install -r requirements.txt
# Chạy ml agents để training
- mlagents-learn config/ppo/3DBall.yaml --run-id=first3DBallRun

*run id là unique name cho training session*
