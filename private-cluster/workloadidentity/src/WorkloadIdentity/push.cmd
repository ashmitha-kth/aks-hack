
set tag=1.2

docker build -t "acrargoazdev.azurecr.io/workloadidentity/workloadidentity:%tag%" .

docker push "acrargoazdev.azurecr.io/workloadidentity/workloadidentity:%tag%"