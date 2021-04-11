Создаем виртуальный сетевой свич и привязываем его к езернету для доступа виртуалки в интернет
Get-NetAdapter
New-VMSwitch -name MinikubeSwitch  -NetAdapterName "Ethernet 3"  -AllowManagementOS $true

стартуем виртуалку миникуба с коннектом до нашего нового виртуального свича
minikube start vm-driver Hyper-v cpus 4 memory 4096 hyperv-virtual-switch "MinikubeSwitch"

Переключить консоль в контекст миникуба
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

