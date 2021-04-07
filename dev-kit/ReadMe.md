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

