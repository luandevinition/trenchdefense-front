﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class ExtensionTest {

	[Test]
	public void LineSegmentsIntersect()
	{
		Vector2 intersection;
		var actual = Extensions.LineSegmentsIntersection(
			new Vector2(0, 0),
			new Vector2(5, 5),
			new Vector2(0, 5),
			new Vector2(5, 0),
			out intersection);

		Assert.IsTrue(actual);
		Assert.AreEqual(new Vector2(2.5f, 2.5f), intersection);
		
	 	actual = Extensions.LineSegmentsIntersection(
			new Vector2(0, 0),
			new Vector2(2, 0),
			new Vector2(1, 1),
			new Vector2(2, 0),
			out intersection);

		Assert.IsTrue(actual);
		Assert.AreEqual(new Vector2(2f, 0f), intersection);
	}

	[Test]
	public void LineSegmentsDoNotIntersect()
	{
		Vector2 intersection;
		var actual = Extensions.LineSegmentsIntersection(
			new Vector2(3, 0),
			new Vector2(3, 4),
			new Vector2(0, 0),
			new Vector2(1, 1),
			out intersection);

		Assert.IsFalse(actual);
	}
}
