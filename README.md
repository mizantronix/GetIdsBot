It's a simply telegram bot for getting IDs
You can get your own ID or you can get chat ID (need to add bot into a chat if it is a group)
Use docker-compose

```yaml
version: '3.6'

services:
  bot:
    container_name: bot
    image: mizantronix/get-id-tgbot
    environment:
      SECRET_TOKEN: {YOUR_TELEGRAM_BOT_TOKEN}
```

or just run with `-e` (env) param:
`docker run -e SECRET_TOKEN={YOUR_TELEGRAM_BOT_TOKEN} mizantronix/get-id-tgbot`