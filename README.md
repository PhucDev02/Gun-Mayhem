## Python 3.10.9 
# Cài đặt môi trường python: 
## C1
- mở cmd tại thư mục game
- python -m venv venv
- .\venv\Scripts\activate
- pip install -r requirements.txt
## C2
- mở cmd tại thư mục game
- python -m venv venv
- .\venv\Scripts\activate
- pip install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
- pip install mlagents==1.1.0
# Chạy ml agents để training
- mlagents-learn config/ppo/3DBall.yaml --run-id=first3DBallRun

*run id là unique name cho training session*
# Mở tensorboard
- tensorboard --logdir results