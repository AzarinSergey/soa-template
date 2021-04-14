—оздаем виртуальный сетевой свич и прив€зываем его к езернету дл€ доступа виртуалки в интернет
Get-NetAdapter
New-VMSwitch -name MinikubeSwitch  -NetAdapterName "Ethernet 3"  -AllowManagementOS $true

стартуем виртуалку миникуба с коннектом до нашего нового виртуального свича
minikube start vm-driver Hyper-v cpus 4 memory 4096 hyperv-virtual-switch "MinikubeSwitch"

ѕереключить консоль в контекст миникуба
& minikube docker-env | Invoke-Expression

из папки с dockerfile-ом запускаем:
docker build --tag soa-template/test-api:develop .

запускаем сервис хелмом:
helm install apitest .\charts\apitest

убиваем сервис хелмом 
helm unistall apitest

смотрим список хелмом
helm list 

смотрим поды
 kubectl get pods

смотрим в миникубе сервисы:
 minikube service list




 —ћќ“–≈“№ Ћќ√» BRIDGE TO KUBERNETES:
Set the environment variable MS_VS_AZUREDEVSPACES_TOOLS_LOGGING_ENABLED=true
Open Visual Studio and run your scenario
Provide logs from:
%TEMP%/Bridge to Kubernetes
%temp%\Microsoft.VisualStudio.Kubernetes.Debugging



<service-name>.<namespace>.svc.cluster.local:<service-port>
api-test.default.svc.cluster.local