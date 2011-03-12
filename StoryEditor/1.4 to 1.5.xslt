<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <!--StoryProject@version changed from 1.4 to 1.5-->
  <xsl:template match="StoryProject/@version">
    <xsl:attribute name="version">
      <xsl:text>1.5</xsl:text>
    </xsl:attribute>
  </xsl:template>

  <!--Add new DefaultAllowed and DefaultRequired attributes to the Member element-->
  <xsl:template match="Member">
    <Member>
      <xsl:apply-templates select="@*"/>
      <xsl:choose>
        <xsl:when test="@memberType = 'ProjectFacilitator'">
          <xsl:attribute name="DefaultTasksAllowed">
            <xsl:if test="following::Languages/LanguageInfo[@lang='Vernacular']">
              <xsl:text>VernacularLangFields, </xsl:text>
            </xsl:if>
            <xsl:if test="following::Languages/LanguageInfo[@lang='NationalBt']">
              <xsl:text>NationalBtLangFields, </xsl:text>
            </xsl:if>
            <xsl:if test="following::Languages/LanguageInfo[@lang='InternationalBt']">
              <xsl:text>InternationalBtFields, </xsl:text>
            </xsl:if>
            <xsl:if test="following::Languages/LanguageInfo[@lang='FreeTranslation']">
              <xsl:text>FreeTranslationFields, </xsl:text>
            </xsl:if>
            <xsl:text>Anchors, Retellings, TestQuestions, Answers</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="DefaultTasksRequired">Anchors</xsl:attribute>
        </xsl:when>
        <xsl:when test="@memberType = 'ConsultantInTraining'">
          <xsl:attribute name="DefaultTasksAllowed">SendToProjectFacilitatorForRevision, SendToCoachForReview</xsl:attribute>
          <xsl:attribute name="DefaultTasksRequired">SendToCoachForReview</xsl:attribute>
        </xsl:when>
      </xsl:choose>
    </Member>
  </xsl:template>

  <!--Add the same new attributes to the story element-->
  <xsl:template match="story">
    <story>
      <xsl:apply-templates select="@*"/>
      <xsl:attribute name="TasksAllowedPf">
        <xsl:if test="preceding::Languages/LanguageInfo[@lang='Vernacular']">
          <xsl:text>VernacularLangFields, </xsl:text>
        </xsl:if>
        <xsl:if test="preceding::Languages/LanguageInfo[@lang='NationalBt']">
          <xsl:text>NationalBtLangFields, </xsl:text>
        </xsl:if>
        <xsl:if test="preceding::Languages/LanguageInfo[@lang='InternationalBt']">
          <xsl:text>InternationalBtFields, </xsl:text>
        </xsl:if>
        <xsl:if test="preceding::Languages/LanguageInfo[@lang='FreeTranslation']">
          <xsl:text>FreeTranslationFields, </xsl:text>
        </xsl:if>
        <xsl:text>Anchors, Retellings, TestQuestions, Answers</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="TasksRequiredPf">Anchors</xsl:attribute>
      <xsl:attribute name="TasksAllowedCit">SendToProjectFacilitatorForRevision, SendToCoachForReview</xsl:attribute>
      <xsl:attribute name="TasksRequiredCit">SendToCoachForReview</xsl:attribute>
      <xsl:apply-templates select="node()"/>
    </story>
  </xsl:template>

  <!--Tests was changed to TestsRetellings and an identical TestsTqAnswers-->
  <xsl:template match="Tests">
    <TestsRetellings>
      <xsl:for-each select="Test">
        <TestRetelling>
          <xsl:attribute name="memberID">
            <xsl:value-of select="@memberID"/>
          </xsl:attribute>
        </TestRetelling>
      </xsl:for-each>
    </TestsRetellings>
    <TestsTqAnswers>
      <xsl:for-each select="Test">
        <TestTqAnswer>
          <xsl:attribute name="memberID">
            <xsl:value-of select="@memberID"/>
          </xsl:attribute>
        </TestTqAnswer>
      </xsl:for-each>
    </TestsTqAnswers>
  </xsl:template>

</xsl:stylesheet>
