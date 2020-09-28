# bot.py
import discord
import asyncio




class DiscordBot():
    client = discord.Client()

    async def start_client_loop(self):
        """Login and start the client loop as new task
           NOP if client is already open

        Returns:
            [bool]: True on sucess
        """
        if self.client.is_closed:

            loop = asyncio.get_event_loop()


            try:
                await self.client.login('NDQ2MDUyMTcxNTAzMzcwMjQy.WvtIdA.7guJsBmPccwKlIxJFGRi8eVMkGoaaaa')
            except discord.errors.LoginFailure:
                print('Invalid token')
                return False
            except discord.errors.HTTPException:
                print('Connection error')
                return False
            else:
                loop.create_task(self.client.connect())
                return  True



    async def stop_client_loop(self):
        if not self.client.is_closed:
            await self.client.close()
            await self.client.logout()
        


    async def send_message(self):
        channel = self.client.get_channel(482524200892891158)
        await channel.send('XYZ')

    


