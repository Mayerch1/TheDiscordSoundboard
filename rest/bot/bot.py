# bot.py
import discord
import asyncio




class DiscordBot():
    
    def __init__(self):
        self.client = discord.Client()
        self.started = False


    async def start_client_loop(self, token):
        """Login and start the client loop as new task
           NOP if client is already open

           throws only for invalid token (discord.errors.LoginFailure)

        Returns:
            [bool]: True on sucess
        """
        if not self.started:

            loop = asyncio.get_event_loop()

            try:
                await self.client.login(token)
            except discord.errors.LoginFailure as e:
                print('Invalid token')
                raise e
            except discord.errors.HTTPException:
                print('Connection error')
                return False
            else:
                loop.create_task(self.client.connect())
                self.started = True
                return  True
        else:
            return True



    async def stop_client_loop(self):
        if self.started:
            await self.client.close()
            await self.client.logout()
            self.started = False
        


    async def send_message(self):
        channel = self.client.get_channel(482524200892891158)
        await channel.send('XYZ')

    


