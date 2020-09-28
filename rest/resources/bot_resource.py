import quart
from quart import jsonify

from bot.bot import DiscordBot

from dao.buttonDao import ButtonDao

bot = DiscordBot()
bot_page = quart.Blueprint('bot_page', __name__, template_folder='templates')



@bot_page.route('/api/v1/bot/logout')
async def logout():
    await bot.stop_client_loop()
    return 'OK', 200




@bot_page.route('/api/v1/bot/login')
async def login():
    await bot.start_client_loop()
    return 'OK', 200







