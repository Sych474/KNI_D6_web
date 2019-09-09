cd ./kni_d6
if docker-compose up -d; then
  echo -e "\n[INFO] Stack was successfully deployed."
else
    echo -e "\n[ERROR] Couldn't deploy docker stack."
  exit 4
fi
