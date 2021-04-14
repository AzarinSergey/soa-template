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




 �������� ���� BRIDGE TO KUBERNETES:
Set the environment variable MS_VS_AZUREDEVSPACES_TOOLS_LOGGING_ENABLED=true
Open Visual Studio and run your scenario
Provide logs from:
%TEMP%/Bridge to Kubernetes
%temp%\Microsoft.VisualStudio.Kubernetes.Debugging



<service-name>.<namespace>.svc.cluster.local:<service-port>
api-test.default.svc.cluster.local