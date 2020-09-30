from model.botState import BotState
from model.botTrackData import BotTrackData
from model.config import Config

from dao.configDao import ConfigDao

from bot.bot import DiscordBot


class BotStateDao:
    state_dao = None

    # stores the trackdata
    # could be replaced by database
    queue_backend = []

    # only one bot is existing here
    # could be replaced by dict (accessed with config id (PK))
    bot = None




    def __init__(self):
        # do not log-in yet
        self.bot = DiscordBot()
        self.bot_state = BotState()


    @staticmethod
    def get_instance():
        if BotStateDao.state_dao is None:
            BotStateDao.state_dao = BotStateDao()

        return BotStateDao.state_dao



    async def bot_login(self):
        """login the bot with the token of the db
           starts the bot loop

           throws discord.errors.LoginFailure on invalid token

        Returns:
            bool: True on sucess
        """
        cfg = ConfigDao.get_instance().get_config(1)
        return await self.bot.start_client_loop(cfg.bot_token)
    


    async def bot_logout(self):
        # only logoff if client is not active
        return await self.bot.stop_client_loop()




    


