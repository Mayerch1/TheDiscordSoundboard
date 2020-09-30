import quart
from quart import jsonify

import discord.errors

from dao.botStateDao import BotStateDao




bot_page = quart.Blueprint('bot_page', __name__, template_folder='templates')



@bot_page.route('/api/v1/bot/logout')
async def logout():
    await bot.stop_client_loop()
    return 'OK', 200



@bot_page.route('/api/v1/bot/login')
async def login():

    try:
        sucess = await BotStateDao.get_instance().bot_login()
    except discord.errors.LoginFailure:
        return 'Invalid Token', 403

    if not sucess:
        return 'Networking error', 503
    else:
        return 'OK', 200








