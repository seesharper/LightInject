using System;

namespace DependencyInjector.Tests
{
	public class Gun
	{
		private readonly Func<Bullet> bullets;

		public Gun(Game game, Func<Bullet> bullets)
		{
			this.bullets = bullets;
		}

		public void Shoot()
		{
			var bullet = bullets();
			bullet.ToString();
		}
	}
}