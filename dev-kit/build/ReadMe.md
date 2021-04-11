������� ����������� ������� ���� � ����������� ��� � �������� ��� ������� ��������� � ��������
Get-NetAdapter
New-VMSwitch -name MinikubeSwitch  -NetAdapterName "Ethernet 3"  -AllowManagementOS $true

�������� ��������� �������� � ��������� �� ������ ������ ������������ �����
minikube start vm-driver Hyper-v cpus 4 memory 4096 hyperv-virtual-switch "MinikubeSwitch"

����������� ������� � �������� ��������
& minikube docker-env | Invoke-Expression

�� ����� � dockerfile-�� ���������:
docker build --tag soa-template/test-api:develop .

��������� ������ ������:
helm install apitest .\charts\apitest

������� ������ ������ 
helm unistall apitest

������� ������ ������
helm list 

������� ����
 kubectl get pods

������� � �������� �������:
 minikube service list

