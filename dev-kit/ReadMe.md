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

